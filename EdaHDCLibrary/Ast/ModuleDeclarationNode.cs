﻿namespace MikroPic.EdaTools.v1.Hdc.Ast {

    public sealed class ModuleDeclarationNode: DeclarationNode {

        public ModuleDeclarationNode(string name):
            base(name) {
        }

        public override void AcceptVisitor(IVisitor visitor) {
        }
    }
}
