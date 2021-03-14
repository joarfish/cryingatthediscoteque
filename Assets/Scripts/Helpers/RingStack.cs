using System;

namespace Helpers {
    public class RingStack<T> {
        private readonly T[] _elements;
        private readonly int _capacity;
        private int _lastIndex;
        private int _firstIndex;
        private int _elementCount;

        public RingStack(int capacity) {
            _elements = new T[capacity];
            _lastIndex = 0;
            _firstIndex = 0;
            _capacity = capacity;
            _elementCount = 0;
        }

        public void Push(T element) {
            _elements[_lastIndex] = element;
            _lastIndex = (_lastIndex + 1) % _capacity;
            if (_elementCount == _capacity) {
                _firstIndex = (_firstIndex + 1) % _capacity;
            }

            if (_firstIndex == _lastIndex) {
                _firstIndex = (_firstIndex + 1) % _capacity;
            }

            _elementCount = (_elementCount == _capacity) ? _elementCount : _elementCount + 1;
        }

        public T Pop() {
            if (_lastIndex == _firstIndex) {
                throw new Exception("Cannot call pop on an empty array!");
            }

            _lastIndex--;
            _elementCount--;

            if (_lastIndex == -1) {
                _lastIndex = _capacity - 1;
            }

            if (_lastIndex == _firstIndex && _elementCount > 0) {
                _firstIndex--;
                if (_firstIndex == -1) {
                    _firstIndex = _capacity - 1;
                }
            }

            return _elements[_lastIndex];
        }

        public bool HasElements() {
            return _elementCount != 0;
        }

        public override string ToString() {
            var r = "_lastIndex=" + _lastIndex + " _firstIndex=" + _firstIndex + " _elementCount=" + _elementCount +
                    "\n";

            for (var i = 0; i < _elements.Length; i++) {
                r += i + ": " + _elements[i] + "\n";
            }

            return r;
        }
    }
}