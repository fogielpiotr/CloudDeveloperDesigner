import { fabric } from 'fabric';
import { DiagramCalculation } from './diagram.calulation';
import { ARROW_HEIGHT, ARROW_WIDTH, LINE_COLOR, NODE_HEIGHT, NODE_WIDTH } from './diagram.defaults';

export class DiagramShapeCrator {
    static createImageNode(image: fabric.Image, event: any): fabric.Image {
        image.scaleToHeight(NODE_HEIGHT);
        image.scaleToWidth(NODE_WIDTH);
        image.left = event.layerX;
        image.top = event.layerY;
        image.crossOrigin = "Anonymous";
        image.hasControls = false;
        image.toObject = (function (toObject) {
            return function () {
                return fabric.util.object.extend(toObject.call(this), {
                    name: this.name,
                    data: this.data
                });
            };
        })(image.toObject);

        return image;
    }

    static createLineNode(coords: number[]): fabric.Line {
        var line = new fabric.Line(coords, {
            fill: LINE_COLOR,
            stroke: LINE_COLOR,
            strokeWidth: 0.5,
            selectable: false,
            cornerStyle: "rect",
        });

        line.toObject = (function (toObject) {
            return function () {
                return fabric.util.object.extend(toObject.call(this), {
                    name: this.name,
                    data: this.data
                });
            };
        })(line.toObject);

        return line;
    }

    static createLineArrow(line: fabric.Line): fabric.Triangle {
        const arrow = new fabric.Triangle({
            left: line.x2,
            top: line.y2,
            angle: DiagramCalculation.calculateArrowAngle(line.x1, line.y1, line.x2, line.y2),
            originX: 'center',
            originY: 'center',
            hasBorders: false,
            hasControls: false,
            lockScalingX: true,
            lockScalingY: true,
            lockRotation: true,
            width: ARROW_WIDTH,
            height: ARROW_HEIGHT,
            fill: LINE_COLOR,
            selectable: false,
        });

        arrow.toObject = (function (toObject) {
            return function () {
                return fabric.util.object.extend(toObject.call(this), {
                    name: this.name,
                    data: this.data
                });
            };
        })(arrow.toObject);

        return arrow;
    }

    static createTextNode(name: string): fabric.Text {
        const text = new fabric.Text(name,
            {
                fontSize: 15,
                selectable: false,
                hasBorders: false,
                hasControls: false,
                lockScalingX: true,
                lockScalingY: true,
                lockRotation: true
            });
        text.toObject = (function (toObject) {
            return function () {
                return fabric.util.object.extend(toObject.call(this), {
                    name: this.name,
                    data: this.data
                });
            };
        })(text.toObject);

        return text;
    }
}