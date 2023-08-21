import { TextDiagramObject } from './../../diagram/shared/diagram-object.model';
import { EventService } from './../services/event-service';
import { Injectable } from "@angular/core";
import { fabric } from 'fabric';
import { Guid } from "guid-typescript";
import { AppDiagramObject, ArrowDiagramObject, ResourceDiagramObject, DiagramObject, LineDiagramObject } from "../../diagram/shared/diagram-object.model";
import { DiagramObjectType } from "./diagram-object-type-enum";
import { DiagramCalculation } from "./diagram.calulation";
import { DiagramShapeCrator } from "./diagram.shape-crator";
import { MAX_ZOOM, MIN_ZOOM, NODE_HEIGHT, NODE_WIDTH } from "./diagram.defaults";

@Injectable({
    providedIn: 'root'
})
export class DiagramManager {
    private currentDiagram: fabric.Canvas;
    private activeObjectId: string;
    private diagramInitialWidth: number;
    private diagramInitialHeight: number;

    constructor(private eventService: EventService) { }

    initializeDiagram(canvasElementId: string, width: number, height: number) {
        this.currentDiagram = new fabric.Canvas(canvasElementId);
        this.diagramInitialHeight = height;
        this.diagramInitialWidth = width;
        this.currentDiagram.setDimensions({ width: this.diagramInitialWidth, height: this.diagramInitialHeight });
        this.currentDiagram.renderAll();

        this.currentDiagram.on('mouse:down', (options) => {
            if (options.target && options.target.type === 'image') {
                this.activeObjectId = options.target.data.id;
            } else {
                this.activeObjectId = null;
            }
            this.eventService.DiagramSelectionChanged.emit(this.activeObjectId);
        });

        this.currentDiagram.on('object:moving', (e: fabric.IEvent<MouseEvent>) => {
            const zoom = this.currentDiagram.getZoom();
            const targetletf = e.target.left;
            const diagramWidtth = this.currentDiagram.width / zoom;
            const targetTop = e.target.top;
            const diagramHeight = this.currentDiagram.height / zoom;
            if (e.target.left < 0) {
                e.target.left = 0;
            }
            else if (targetletf + 10 > diagramWidtth) {
                e.target.left = diagramWidtth - (NODE_WIDTH / zoom);
            }
            if (targetTop < 0) {
                e.target.top = 0;
            }
            else if (targetTop + 10 > diagramHeight) {
                e.target.top = diagramHeight - (NODE_HEIGHT / zoom);
            }
            const movedNode = e.target as fabric.Image;
            const movedNodeData = movedNode.data as DiagramObject;
            if (movedNodeData) {
                if (movedNodeData.type === DiagramObjectType.Application || movedNodeData.type === DiagramObjectType.Resource) {
                    const textNode = this.getNodeById(this.getTextNodes().find(x => x.nodeId == movedNodeData.id).id) as fabric.Text;
                    DiagramCalculation.calculateTextNodePosition(textNode, movedNode);
                    this.getLinesNodes().filter(x => x.from === movedNodeData.id).forEach(x => {
                        const fromNode = movedNode;
                        const from = fromNode.getCenterPoint();
                        const toNode = this.getNodeById(x.to)
                        const to = toNode.getCenterPoint();
                        const arrowNode = this.getArrowNodes().find(a => a.lineId === x.id);
                        let arrow = this.getNodeById(arrowNode.id);
                        let line = this.getNodeById(x.id);

                        this.currentDiagram.remove(arrow);
                        this.currentDiagram.remove(line);
                        const [fromCords, toCords] = DiagramCalculation.calculateLineCoords(from, to);
                        line = DiagramShapeCrator.createLineNode([fromCords.x, fromCords.y, toCords.x, toCords.y]);
                        const lineId = Guid.create().toString();
                        line.data = {
                            id: lineId,
                            type: DiagramObjectType.Line,
                            from: fromNode.data.id,
                            to: toNode.data.id
                        } as LineDiagramObject;
                        this.currentDiagram.add(line);
                        this.currentDiagram.sendToBack(line);
                        arrow = DiagramShapeCrator.createLineArrow(line as fabric.Line);
                        arrow.data = {
                            id: Guid.create().toString(),
                            lineId: lineId,
                            type: DiagramObjectType.Arrow,
                        } as ArrowDiagramObject;
                        this.currentDiagram.add(arrow);
                        this.currentDiagram.sendToBack(arrow);
                    });

                    this.getLinesNodes().filter(x => x.to === movedNodeData.id).forEach(x => {
                        const fromNode = this.getNodeById(x.from);
                        const from = fromNode.getCenterPoint();
                        const toNode = movedNode;
                        const to = movedNode.getCenterPoint();
                        const arrowNode = this.getArrowNodes().find(a => a.lineId === x.id);
                        let arrow = this.getNodeById(arrowNode.id);
                        let line = this.getNodeById(x.id);

                        this.currentDiagram.remove(arrow);
                        this.currentDiagram.remove(line);
                        const [fromCords, toCords] = DiagramCalculation.calculateLineCoords(from, to);
                        line = DiagramShapeCrator.createLineNode([fromCords.x, fromCords.y, toCords.x, toCords.y]);
                        const lineId = Guid.create().toString();
                        line.data = {
                            id: lineId,
                            type: DiagramObjectType.Line,
                            from: fromNode.data.id,
                            to: toNode.data.id
                        } as LineDiagramObject;
                        this.currentDiagram.add(line);
                        this.currentDiagram.sendToBack(line);
                        arrow = DiagramShapeCrator.createLineArrow(line as fabric.Line);
                        arrow.data = {
                            id: Guid.create().toString(),
                            lineId: lineId,
                            type: DiagramObjectType.Arrow,
                        } as ArrowDiagramObject;
                        this.currentDiagram.add(arrow);
                        this.currentDiagram.sendBackwards(arrow);
                    });
                }
            }
        })
    }

    renderDiagram() {
        this.currentDiagram.renderAll();
    }

    getZoom(): number {
        return this.currentDiagram.getZoom();
    }

    setZoom(zoom: number) {
        zoom = zoom > MAX_ZOOM ? MAX_ZOOM : zoom;
        zoom = zoom < MIN_ZOOM ? MIN_ZOOM : zoom;
        this.currentDiagram.setZoom(zoom);
        if (zoom >= 1) {
            this.currentDiagram.setDimensions(
                {
                    width: this.diagramInitialWidth * zoom,
                    height: this.diagramInitialHeight * zoom
                });
        }
        else {
            this.currentDiagram.setDimensions(
                {
                    width: this.diagramInitialWidth,
                    height: this.diagramInitialHeight
                });
        }
    }

    getImage(): string {
        this.currentDiagram.backgroundColor = "white";
        return this.currentDiagram.toDataURL({
            format: 'jpeg',
            quality: 1,
            multiplier: 2,
        });
    }

    getActiveObjectId(): string | null {
        return this.activeObjectId;
    }

    getActiveNode(): fabric.Object {
        return this.currentDiagram.getActiveObject();
    }

    getNodeByName(name: string): fabric.Object {
        return this.currentDiagram.getObjects().filter(x => {
            if (x.data) {
                if (x.data.values) {
                    if (x.data.values['name'] === name) {
                        return true;
                    }
                    return false;
                }
                if (x.data.name && x.data.name === name) {
                    return true;
                }
                return false;
            }

            return false;
        })[0]
    }

    getNodeById(id: string): fabric.Object {
        return this.currentDiagram.getObjects().find(x => x.data.id === id);
    }

    getSelectableObject(): fabric.Object[] {
        return this.currentDiagram.getObjects().filter(x => x.selectable);
    }

    getAppNodeData(): AppDiagramObject {
        return this.currentDiagram.getActiveObject()?.data as AppDiagramObject;
    }

    getResourceNodeData(): ResourceDiagramObject {
        return this.currentDiagram.getActiveObject()?.data as ResourceDiagramObject;
    }

    getActiveNodeId(): string {
        return this.currentDiagram.getActiveObject()?.data.id;
    }

    getActiveNodeType(): DiagramObjectType | null {
        return (this.currentDiagram.getActiveObject()?.data as DiagramObject)?.type;
    }

    remove(): void {
        this.getLinesNodes().filter(x => x.from === this.activeObjectId || x.to === this.activeObjectId).forEach(line => {
            const lineNode = this.getNodeById(line.id);

            this.getArrowNodes().filter(x => x.lineId === line.id).forEach(arrow => {
                const arrowLine = this.getNodeById(arrow.id);
                this.currentDiagram.remove(arrowLine);
            });
            this.currentDiagram.remove(lineNode);
        });
        this.currentDiagram.remove(this.getNodeById(this.getTextNodes().find(x => x.nodeId == this.activeObjectId).id));

        this.activeObjectId = null;
        this.currentDiagram.remove(this.currentDiagram.getActiveObject());
    }

    onResize(): void {
        if (this.currentDiagram) {
            this.currentDiagram.setDimensions({ width: document.getElementById("diagram-wrapper").offsetWidth, height: document.getElementById("diagram-wrapper").offsetHeight });
            this.currentDiagram.renderAll();
        }
    }

    addNode(e: any, data: ResourceDiagramObject | AppDiagramObject, displayName: string): void {
        fabric.Image.fromURL(data.iconFile, (image) => {
            const node = DiagramShapeCrator.createImageNode(image, e);
            node.srcFromAttribute = true;
            this.currentDiagram.add(node);
            image.data = data;

            const text = DiagramShapeCrator.createTextNode(displayName);
            text.data = {
                id: Guid.create().toString(),
                type: DiagramObjectType.Text,
                nodeId: data.id
            } as TextDiagramObject;

            this.currentDiagram.add(text);
            this.currentDiagram.bringForward(text);
            DiagramCalculation.calculateTextNodePosition(text, image);
        });
    }

    getAppNodes(): Array<AppDiagramObject> {
        const filtered = this.currentDiagram.getObjects().filter(x => (x.data as DiagramObject)?.type === DiagramObjectType.Application);
        return filtered.map(x => { return x.data as AppDiagramObject });
    };

    getResourceNodes(): Array<ResourceDiagramObject> {
        const filtered = this.currentDiagram.getObjects().filter(x => (x.data as DiagramObject)?.type === DiagramObjectType.Resource);
        return filtered.map(x => { return x.data as ResourceDiagramObject });
    };

    getLinesNodes(): Array<LineDiagramObject> {
        const filtered = this.currentDiagram.getObjects().filter(x => (x.data as DiagramObject)?.type === DiagramObjectType.Line);
        return filtered.map(x => { return x.data as LineDiagramObject });
    };

    getTextNodes(): Array<TextDiagramObject> {
        const filtered = this.currentDiagram.getObjects().filter(x => (x.data as DiagramObject)?.type === DiagramObjectType.Text);
        return filtered.map(x => { return x.data as TextDiagramObject });
    };

    getArrowNodes(): Array<ArrowDiagramObject> {
        const filtered = this.currentDiagram.getObjects().filter(x => (x.data as DiagramObject)?.type === DiagramObjectType.Arrow);
        return filtered.map(x => { return x.data as ArrowDiagramObject });
    };

    getDiagramJson(): string {
        return this.currentDiagram.toJSON();
    }

    loadDiagramFromJson(json: string) {
        this.currentDiagram.loadFromJSON(json, this.currentDiagram.renderAll.bind(this.currentDiagram),
            (loadedObject: object, createdObject: fabric.Object) => {
                createdObject.toObject = (function (toObject) {
                    return function () {
                        return fabric.util.object.extend(toObject.call(this), {
                            name: this.name,
                            data: this.data
                        });
                    };
                })(createdObject.toObject);
                createdObject.data = (loadedObject as fabric.Object).data;
                createdObject.hasControls = false;
                var diagramObjectData = createdObject.data as DiagramObject;
                if (diagramObjectData.type === DiagramObjectType.Arrow ||
                    diagramObjectData.type === DiagramObjectType.Line ||
                    diagramObjectData.type === DiagramObjectType.Text) {
                    createdObject.selectable = false;
                }
            });
    }

    clearDiagram() {
        this.currentDiagram.getObjects().forEach(x => {
            this.currentDiagram.remove(x);
        });
    }

    connectObjects(fromNode: fabric.Object, toNode: fabric.Object): void {
        const from = fromNode.getCenterPoint();
        const to = toNode.getCenterPoint();
        const existingConnection = this.getLinesNodes().find(x => x.from == fromNode.data.id && x.to == toNode.data.id);
        if (existingConnection) {
            return;
        }
        const [fromCalc, toCalc] = DiagramCalculation.calculateLineCoords(from, to);
        var line = DiagramShapeCrator.createLineNode([fromCalc.x, fromCalc.y, toCalc.x, toCalc.y]);
        const lineId = Guid.create().toString();
        line.data = {
            id: lineId,
            type: DiagramObjectType.Line,
            from: fromNode.data.id,
            to: toNode.data.id
        } as LineDiagramObject;
        this.currentDiagram.add(line);
        this.currentDiagram.sendToBack(line);
        let arrow = DiagramShapeCrator.createLineArrow(line);
        arrow.data = {
            id: Guid.create().toString(),
            lineId: lineId,
            type: DiagramObjectType.Arrow,
        } as ArrowDiagramObject;
        this.currentDiagram.add(arrow);
        this.currentDiagram.sendToBack(arrow);
    }

    removeConnection(fromNode: fabric.Object, toNode: fabric.Object) {
        const lineData = this.getLinesNodes().find(x => x.from == fromNode.data.id && x.to == toNode.data.id);
        const arrowData = this.getArrowNodes().find(x => x.lineId === lineData.id);

        let arrow = this.getNodeById(arrowData.id);
        let line = this.getNodeById(lineData.id);

        this.currentDiagram.remove(line);
        this.currentDiagram.remove(arrow);
    }
}