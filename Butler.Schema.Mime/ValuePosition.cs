using System.Linq;

namespace Butler.Schema.Mime {

    internal struct ValuePosition {

        public ValuePosition(int line, int offset) {
            Line = line;
            Offset = offset;
        }

        public static bool operator ==(ValuePosition pos1, ValuePosition pos2) {
            if (pos1.Line == pos2.Line)
                return pos1.Offset == pos2.Offset;
            return false;
        }

        public static bool operator !=(ValuePosition pos1, ValuePosition pos2) {
            return !(pos1 == pos2);
        }

        public override bool Equals(object rhs) {
            if (!(rhs is ValuePosition))
                return false;
            var valuePosition = (ValuePosition) rhs;
            if (Line == valuePosition.Line)
                return Offset == valuePosition.Offset;
            return false;
        }

        public override int GetHashCode() {
            return Line*1000 + Offset;
        }

        public int Line;
        public int Offset;

    }

}