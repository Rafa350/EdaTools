namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public sealed class EdaDeviceAttribute: EdaAttributeBase {

        public EdaDeviceAttribute(string name, string value = null) :
            base(name, value) { 
        }

        public override void AcceptVisitor(IEdaBoardVisitor visitor) {
            
            throw new System.NotImplementedException();
        }
    }
}
