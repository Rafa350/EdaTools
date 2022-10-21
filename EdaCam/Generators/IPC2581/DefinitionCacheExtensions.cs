using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    internal static class DefinitionCacheExtensions {

        private sealed class Visitor: EdaElementVisitor {

            private readonly DefinitionCache _definitionCache;

            public Visitor(DefinitionCache definitionCache) {

                _definitionCache = definitionCache;
            }

            public override void Visit(EdaLineElement element) {

                _definitionCache.DeclareLineDefinition(element.Thickness);
            }

            public override void Visit(EdaArcElement element) {

                _definitionCache.DeclareLineDefinition(element.Thickness);
            }

            public override void Visit(EdaViaElement element) {

                _definitionCache.DeclareCircleDefinition(element.OuterSize);
                _definitionCache.DeclareCircleDefinition(element.InnerSize);
            }

            public override void Visit(EdaThtPadElement element) {

                _definitionCache.DeclareRectRoundDefinition(element.TopSize, element.CornerRatio);
                _definitionCache.DeclareRectRoundDefinition(element.BottomSize, element.CornerRatio);
                _definitionCache.DeclareRectRoundDefinition(element.InnerSize, element.CornerRatio);
                if (element.MaskClearance > 0) {
                    _definitionCache.DeclareRectRoundDefinition(element.TopSize.Inflated(element.MaskClearance), element.CornerRatio);
                    _definitionCache.DeclareRectRoundDefinition(element.BottomSize.Inflated(element.MaskClearance), element.CornerRatio);
                }
            }

            public override void Visit(EdaSmtPadElement element) {
                
                _definitionCache.DeclareRectRoundDefinition(element.Size, element.CornerRatio);
                if (element.MaskClearance > 0)
                    _definitionCache.DeclareRectRoundDefinition(element.Size.Inflated(element.MaskClearance), element.CornerRatio);
            }
        }

        public static void DeclareDefinitions(this DefinitionCache definitionCache, EdaBoard board) {

            var visitor = new Visitor(definitionCache);
            visitor.Visit(board);
        }
    }
}
