﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortensWay
{
    internal class Button : GameObject<Enum>
    {
        public Button(Enum type, Vector2 spawnPos) : base(type, spawnPos)
        {

        }

        public override void LoadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }

        //public void Update()
        //{
        //    if (Rectangle.Contains(new Point(MousePointer.GetState().X, Mouse.GetState().Y)) && Mouse.GetState().LeftButton == ButtonState.Pressed)
        //    {
        //        OnClick();
        //    }

        //}

        //public void OnClick()
        //{
        //    GameWorld.Instance.OnButtonClick(button);


        //}
    }
}
