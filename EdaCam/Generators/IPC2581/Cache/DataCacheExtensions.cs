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

            public override void Visit(EdaCircleElement element) {

                if (element.Filled)
                    _cache.AddFillDescEntry(true);
                else
                    _cache.AddLineDescEntry(element.Thickness, EdaLineCap.Flat);
            }

            public override void Visit(EdaViaElement element) {

                _cache.AddCircleEntry(element.OuterSize);
                _cache.AddCircleEntry(element.InnerSize);
            }

            public override void Visit(EdaThtPadElement element) {

                bool flat = element.CornerShape == EdaThtPadElement.CornerShapeType.Flat;

                _cache.AddRectEntry(element.TopSize, element.CornerRatio, flat);
                _cache.AddRectEntry(element.BottomSize, element.CornerRatio, flat);
                _cache.AddRectEntry(element.InnerSize, element.CornerRatio, flat);
                if (element.MaskClearance > 0) {
                    _cache.AddRectEntry(element.TopSize.Inflated(element.MaskClearance), element.CornerRatio, flat);
                    _cache.AddRectEntry(element.BottomSize.Inflated(element.MaskClearance), element.CornerRatio, flat);
                }
            }

            public override void Visit(EdaSmtPadElement element) {

                bool flat = element.CornerShape == EdaSmtPadElement.CornerShapeType.Flat;

                _cache.AddRectEntry(element.Size, element.CornerRatio, flat);
                if (element.MaskClearance > 0)
                    _cache.AddRectEntry(element.Size.Inflated(element.MaskClearance), element.CornerRatio, flat);
                if (!element.PasteReductionRatio.IsZero) {
                    var size = element.Size.Deflated(element.PasteReductionRatio);
                    _cache.AddRectEntry(size, element.CornerRatio, flat);
                }
            }

            public override void Visit(EdaTextElement element) {

                _cache.AddLineDescEntry(element.Thickness, EdaLineCap.Round);
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
