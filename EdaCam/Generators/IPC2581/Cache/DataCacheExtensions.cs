using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    internal static class DataCacheExtensions {

        private sealed class Visitor: EdaElementVisitor {

            private readonly DataCache _cache;

            public Visitor(DataCache definitionCache) {

                _cache = definitionCache;
            }

            public override void Visit(EdaLineElement element) {

                _cache.AddLineDescEntry(element.Thickness, element.LineCap);
            }

            public override void Visit(EdaArcElement element) {

                _cache.AddLineDescEntry(element.Thickness, element.LineCap);
            }

            public override void Visit(EdaViaElement element) {

                _cache.AddCircleEntry(element.OuterSize);
                _cache.AddCircleEntry(element.InnerSize);
            }

            public override void Visit(EdaThtPadElement element) {

                _cache.AddRectRoundEntry(element.TopSize, element.CornerRatio);
                _cache.AddRectRoundEntry(element.BottomSize, element.CornerRatio);
                _cache.AddRectRoundEntry(element.InnerSize, element.CornerRatio);
                if (element.MaskClearance > 0) {
                    _cache.AddRectRoundEntry(element.TopSize.Inflated(element.MaskClearance), element.CornerRatio);
                    _cache.AddRectRoundEntry(element.BottomSize.Inflated(element.MaskClearance), element.CornerRatio);
                }
            }

            public override void Visit(EdaSmtPadElement element) {

                _cache.AddRectRoundEntry(element.Size, element.CornerRatio);
                if (element.MaskClearance > 0)
                    _cache.AddRectRoundEntry(element.Size.Inflated(element.MaskClearance), element.CornerRatio);
            }
        }

        public static void AddBoardEntries(this DataCache dataCache, EdaBoard board) {

            var visitor = new Visitor(dataCache);
            visitor.Visit(board);
        }

        public static void AddDefaultEntries(this DataCache dataCache) {

            dataCache.AddFillDescEntry(true);
            dataCache.AddFillDescEntry(false);
        }
    }
}
