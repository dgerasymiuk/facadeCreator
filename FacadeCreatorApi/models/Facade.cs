﻿using System;
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
    public class Facade : Figure
    {
        
        int number=0;
        private Font currentFont;

        public Facade(int number, int width, int height) : base(width, height)
        {
            this.number = number;
        }

        public override Figure copy()
        {
            return new Facade(number, width, height);
        }

        public override void draw(Graphics context, int x, int y)
        {
            string describe = number + "\n" + width + "x" + height;
            if (currentFont == null)
            {
                updateFont(context, describe);
            }
            
            //base.draw(context, x, y);
            context.DrawRectangle(new Pen(Color.Black, LINE_SIZE), x, y, width, height);
            context.DrawString(describe,currentFont, Brushes.Black, x + 2, y + 2);
        }

        private void updateFont(Graphics context,string text)
        {
            int fontSize = 12;
            
            Font font = new Font("Arial", fontSize);
            SizeF size = context.MeasureString(text, font);
            for (; size.Width < width - 20 && size.Height < height - 20; fontSize += 12)
            {
                font = new Font("Arial", fontSize);
                size = context.MeasureString(text, font);
            }
            currentFont = new Font("Arial", fontSize - 12);
        }

        public override Action getAction(int x, int y, bool cntrl)
        {
            if (!isPointInFigure(x, y))
            {
                currentAction = Action.NO;
                return currentAction;
            }
            else
            {
                currentAction = Action.SHIFT;
                return currentAction;
            }
           
        }
        public override string ToString()
        {
            return number.ToString();
        }
        
    }
}
