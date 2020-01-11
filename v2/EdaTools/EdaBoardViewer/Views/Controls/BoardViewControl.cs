namespace EdaBoardViewer.Views.Controls {

    using System.IO;
    using Avalonia.Controls;
    using Avalonia.Media;
    using EdaBoardViewer.Render;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.IO;

    class BoardViewControl : Control {

        public override void Render(DrawingContext context) {

            Board board = null;

            using (Stream stream = new FileStream("board3.xbrd", FileMode.Open, FileAccess.Read, FileShare.None)) {
                BoardStreamReader reader = new BoardStreamReader(stream);
                board = reader.Read();
            }

            var boardRenderer = new BoardRenderer(context);
            boardRenderer.Render(board);
        }
    }
}
