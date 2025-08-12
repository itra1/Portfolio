using System.Collections.Generic;
using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using UI.Canvas.Presenter;
using UnityEngine;

namespace App.Parsing.Handlers
{
    public class ReferenceResolution : CommandLineValuesHandler
    {
        private readonly ICanvasModifying _canvas;
        
        public ReferenceResolution(ICanvasModifying canvas) 
            : base(CommandLineArgumentNames.ReferenceResolution, 2)
        {
            _canvas = canvas;
        }
        
        protected override void Execute(IReadOnlyList<string> values)
        {
            base.Execute(values);
            
            var sizeX = int.Parse(values[0]);
            var sizeY = int.Parse(values[1]);
            
            _canvas.ReferenceResolution = new Vector2(sizeX, sizeY);
        }
    }
}