import { fabric } from 'fabric';
import { ARROW_HEIGHT, ARROW_WIDTH, NODE_HEIGHT, NODE_WIDTH, TEXT_NODE_Y_OFFSET } from './diagram.defaults';

export class DiagramCalculation {
    static calculateArrowAngle(x1: number, y1: number, x2: number, y2: number): number {
        let angle = 0,
            x, y;

        x = (x2 - x1);
        y = (y2 - y1);

        if (x === 0) {
            angle = (y === 0) ? 0 : (y > 0) ? Math.PI / 2 : Math.PI * 3 / 2;
        } else if (y === 0) {
            angle = (x > 0) ? 0 : Math.PI;
        } else {
            angle = (x < 0) ? Math.atan(y / x) + Math.PI : (y < 0) ? Math.atan(y / x) + (2 * Math.PI) : Math.atan(y / x);
        }

        return (angle * 180 / Math.PI + 90);
    }

    static calculateLineCoords(from: fabric.Point, to: fabric.Point): [fabric.Point, fabric.Point] {
        let distanceX,
            distanceY,
            fromX = from.x,
            fromY = from.y,
            toX = to.x,
            toY = to.y,
            calibrateX = NODE_WIDTH / 2 + ARROW_WIDTH / 2,
            calibrateY = NODE_HEIGHT / 2 + ARROW_HEIGHT / 2;


        if (fromX < toX) {
            distanceX = toX - fromX;
        } else {
            distanceX = fromX - toX;
        }

        if (fromY < toY) {
            distanceY = toY - fromY;
        } else {
            distanceY = fromY - toY;
        }

        if (distanceX > distanceY) {

            if (fromX < toX) {
                toX -= calibrateX;
            } else {
                toX += calibrateX;
            }
        } else {
            if (fromY < toY) {
                toY -= calibrateY;
            } else {
                toY += calibrateY;
            }
        }

        return [new fabric.Point(fromX, fromY), new fabric.Point(toX, toY)];
    }

    static calculateTextNodePosition(textNode: fabric.Text, textFor: fabric.Image) {
        const textForNodeCenterPoint = textFor.getCenterPoint();
        textNode.left = textForNodeCenterPoint.x - (textNode.width / 2);
        textNode.top = textForNodeCenterPoint.y + (NODE_HEIGHT / 2) + TEXT_NODE_Y_OFFSET;
    }
}