﻿using FacadeCreatorApi.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacadeCreatorApi.models
{
    [ComVisible(false)]
    public class BkgImage : Figure
    {
        private Stack<BkgImage> savedStates = new Stack<BkgImage>();
        private Bitmap img;
        public BkgImage(Bitmap img) : base(img.Width, img.Height)
        {
            this.img = img;
            updateResolution();
        }

        public override Figure copy()
        {
            BkgImage newBkgImage = new BkgImage(new Bitmap(img));
            newBkgImage.height = this.height;
            newBkgImage.width = this.width;
            newBkgImage.updateResolution();
            return newBkgImage;
        }

        public override void draw(Graphics context, int x, int y)
        {
            context.DrawImage(img,x,y,width,height);
            base.draw(context, x, y);
        }

        public override Action getAction(int x, int y, bool cntrl)
        {
            if (!isPointInFigure(x, y))
            {
                currentAction = Action.NO;
                return currentAction;
            }
            int c = 0;
            Action action = Action.SHIFT;
            if (Math.Abs(x - width) < 5/delta)
            {
                action = Action.SIZE_WE;
                c++;
            }
            if (Math.Abs(y - height) < 5/delta)
            {
                action = Action.SIZE_NS;
                c++;
            }
            if (c == 2) currentAction = Action.SIZE_NWSE;
            //else if (c == 2 && !cntrl) currentAction = Action.SIZE_NWSE_WR;
            else currentAction = action;
            return currentAction;
        }

        internal void inverse()
        {
            img = ImageConversion.inverseBlackWhiteImage(img);
        }

        internal void mirrorHorizontal()
        {
            img.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }

        internal void mirrorVertical()
        {
            img.RotateFlip(RotateFlipType.RotateNoneFlipX);
        }
        public void rotate(RotateType type)
        {
            RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;
            switch (type)
            {
                case RotateType.Clockwise:
                    rotateFlipType = RotateFlipType.Rotate90FlipNone;
                    break;
                case RotateType.Counter_Clockwise:
                    rotateFlipType = RotateFlipType.Rotate270FlipNone;
                    break;
            }
            int tmp = width;
            width = height;
            height = tmp;
            img.RotateFlip(rotateFlipType);
        }
        public enum RotateType
        {
            Clockwise,
            Counter_Clockwise
        }
        public void dispose()
        {
            img.Dispose();
            img = null;
        }
        public override void saveState()
        {
            savedStates.Push((BkgImage)this.copy());
        }

        public override void backToPrevious()
        {
            if (savedStates.Count == 0) return;
            BkgImage returnedState = savedStates.Pop();
            //this.selected = returnedState.selected;
            this.height = returnedState.height;
            this.width = returnedState.width;
            this.img.Dispose();
            this.img = returnedState.img;
            this.updateResolution();
        }
    }
}
