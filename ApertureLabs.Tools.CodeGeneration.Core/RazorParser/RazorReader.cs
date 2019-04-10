using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ApertureLabs.Tools.CodeGeneration.Core.RazorParser
{
    /// <summary>
    /// Represents a reader that provides fast, noncached, forward-only access
    /// to razor-formatted data.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <remarks>
    /// Credit goes to MS - https://github.com/dotnet/corefx/blob/master/src/System.Private.Xml/src/System/Xml/Core/XmlTextReaderImpl.cs.
    /// </remarks>
    [DebuggerDisplay("Type = {NodeType}, Name = {Name}", Name = "{key}")]
    public class RazorReader : IXmlLineInfo, IDisposable
    {
        #region Fields

        private TextReader textReader;

        // current parsing state (aka. scanner data) 
        private ParsingState _ps;

        // parsing function = what to do in the next Read() (3-items-long stack, usually used just 2 level)
        private ParsingFunction _parsingFunction;
        private ParsingFunction _nextParsingFunction;
        private ParsingFunction _nextNextParsingFunction;

        // stack of nodes
        private NodeData[] _nodes;

        // current node
        private NodeData _curNode;

        // current index
        private int _index = 0;

        // attributes info
        private int _curAttrIndex = -1;
        private int _attrCount;
        private int _attrHashtable;
        private int _attrDuplWalkCount;
        private bool _attrNeedNamespaceLookup;
        private bool _fullAttrCleanup;
        private NodeData[] _attrDuplSortingArray;

        // settings
        private bool _normalize;
        private bool _supportNamespaces = true;
        private WhitespaceHandling _whitespaceHandling;
        private bool _ignorePIs;
        private bool _ignoreComments;
        private bool _checkCharacters;
        private int _lineNumberOffset;
        private int _linePositionOffset;
        private bool _closeInput;
        private long _maxCharactersInDocument;
        private long _maxCharactersFromEntities;

        // stack of parsing states (=stack of entities)
        private ParsingState[] _parsingStatesStack;
        private int _parsingStatesStackTop = -1;

        // fragment parsing
        private RazorNodeType _fragmentType = RazorNodeType.Document;
        private XmlParserContext _fragmentParserContext;
        private bool _fragment;

        // incremental read
        private IncrementalReadDecoder _incReadDecoder;
        private IncrementalReadState _incReadState;
        private LineInfo _incReadLineInfo;
        private int _incReadDepth;
        private int _incReadLeftStartPos;
        private int _incReadLeftEndPos;
        private IncrementalReadCharsDecoder _readCharsDecoder;

        // ReadAttributeValue helpers
        private int _attributeValueBaseEntityId;
        private bool _emptyEntityInAttributeResolved;

        // misc
        private bool _addDefaultAttributesAndNormalize;
        private StringBuilder _stringBuilder;
        private bool _rootElementParsed;
        private bool _standalone;
        private int _nextEntityId = 1;
        private ParsingMode _parsingMode;
        private ReadState _readState = ReadState.Initial;
        private bool _afterResetState;
        private int _documentStartBytePos;
        private int _readValueOffset;
        private Encoding _reportedEncoding;

        #endregion

        #region Constructor/Finalizer

        protected RazorReader()
        {
            // Initialize fields.
            _parsingFunction = ParsingFunction.NoData;
            _nextParsingFunction = ParsingFunction.NoData;
            _nextNextParsingFunction = ParsingFunction.NoData;

            // Initialize properties.
            AttributeCount = 0;
            CanReadValueChunk = true;
            Depth = 0;
            EOF = false;
            HasAttributes = false;
            HasValue = false;
            IsEmptyElement = false;
            LineNumber = 0;
            LinePosition = 0;
            Name = String.Empty;
            NodeType = RazorNodeType.None;
            QuoteChar = '"';
            ReadState = ReadState.Initial;
            Value = "";
        }

        ~RazorReader()
        {
            Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// When overridden in a derived class, gets the number of attributes on
        /// the current node.
        /// </summary>
        /// <value>
        /// The attribute count.
        /// </value>
        public virtual int AttributeCount { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="RazorReader"/>
        /// implements the binary content read methods.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can read value chunk; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanReadValueChunk { get; protected set; }

        /// <summary>
        /// When overridden in a derived class, gets the depth of the current
        /// node in the XML document.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        public virtual int Depth { get; protected set; }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating
        /// whether the reader is positioned at the end of the stream.
        /// </summary>
        /// <value>
        ///   <c>true</c> if EOF; otherwise, <c>false</c>.
        /// </value>
        public virtual bool EOF { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the current node has any
        /// attributes.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has attributes; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasAttributes { get; protected set; }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating
        /// whether the current node can have a <see cref="Value"/>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has value; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasValue { get; protected set; }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating
        /// whether the current node is an empty element (for example,
        /// <MyElement/>).
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty element; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsEmptyElement { get; protected set; }

        /// <summary>
        /// When overridden in a derived class, gets the value of the
        /// attribute with the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string this[int i]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the value of the attribute
        /// with the specified <see cref="Name"/>.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string this[string name]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the qualified name of the
        /// current node.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual string Name { get; protected set; }

        /// <summary>
        /// When overridden in a derived class, gets the type of the current
        /// node.
        /// </summary>
        /// <value>
        /// The type of the node.
        /// </value>
        public virtual RazorNodeType NodeType { get; protected set; }

        /// <summary>
        /// When overridden in a derived class, gets the quotation mark
        /// character used to enclose the value of an attribute node.
        /// </summary>
        /// <value>
        /// The quote character.
        /// </value>
        public virtual char QuoteChar { get; protected set; }

        /// <summary>
        /// When overridden in a derived class, gets the state of the reader.
        /// </summary>
        /// <value>
        /// The state of the read.
        /// </value>
        public virtual ReadState ReadState { get; protected set; }

        /// <summary>
        /// Gets the XmlReaderSettings object used to create this
        /// <see cref="RazorReaderSettings"/> instance.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public virtual RazorReaderSettings Settings { get; protected set; }

        /// <summary>
        /// When overridden in a derived class, gets the text value of the
        /// current node.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public virtual string Value { get; protected set; }

        /// <summary>
        /// Gets The Common Language Runtime (CLR) type for the current node.
        /// </summary>
        /// <value>
        /// The type of the value.
        /// </value>
        public virtual Type ValueType { get; protected set; }

        private bool InAttributeValueIterator
        {
            get
            {
                return _attrCount > 0 && _parsingFunction >= ParsingFunction.InReadAttributeValue;
            }
        }

        public int LineNumber { get; protected set; }

        public int LinePosition { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="RazorReader"/> instance using the
        /// specified stream with default settings.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">stream</exception>
        public static RazorReader Create(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return InitializeRazorReader(
                new StreamReader(stream),
                new RazorReaderSettings());
        }

        /// <summary>
        /// Creates a new <see cref="RazorReader"/> instance with the
        /// specified stream and settings.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// stream
        /// or
        /// settings
        /// </exception>
        public static RazorReader Create(Stream stream,
            RazorReaderSettings settings)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            else if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return InitializeRazorReader(new StreamReader(stream), settings);
        }

        /// <summary>
        /// Creates a new <see cref="RazorReader"/> instance.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">reader</exception>
        public static RazorReader Create(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            return InitializeRazorReader(reader, new RazorReaderSettings());
        }

        /// <summary>
        /// Creates a new <see cref="RazorReader"/> instance
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// reader
        /// or
        /// reader
        /// </exception>
        public static RazorReader Create(TextReader reader,
            RazorReaderSettings settings)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            else if (settings == null)
                throw new ArgumentNullException(nameof(reader));

            return InitializeRazorReader(reader, settings);
        }

        private static RazorReader InitializeRazorReader(TextReader reader,
            RazorReaderSettings settings)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            else if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var razorReader = new RazorReader
            {
                Settings = settings,
                textReader = reader,
            };

            return razorReader;
        }

        /// <summary>
        /// Returns a value indicating whether the string argument is a valid
        /// XML name.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        ///   <c>true</c> if the specified string is name; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static bool IsName(string str)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a value indicating whether or not the string argument is a
        /// valid XML name token.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        ///   <c>true</c> if [is name token] [the specified string]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static bool IsNameToken(string str)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// StripSpaces removes spaces at the beginning and at the end of the
        /// value and replaces sequences of spaces with a single space.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal static string StripSpaces(string value)
        {
            int len = value.Length;
            if (len <= 0)
            {
                return string.Empty;
            }

            int startPos = 0;
            StringBuilder norValue = null;

            while (value[startPos] == 0x20)
            {
                startPos++;
                if (startPos == len)
                {
                    return " ";
                }
            }

            int i;
            for (i = startPos; i < len; i++)
            {
                if (value[i] == 0x20)
                {
                    int j = i + 1;
                    while (j < len && value[j] == 0x20)
                    {
                        j++;
                    }
                    if (j == len)
                    {
                        if (norValue == null)
                        {
                            return value.Substring(startPos, i - startPos);
                        }
                        else
                        {
                            norValue.Append(value, startPos, i - startPos);
                            return norValue.ToString();
                        }
                    }
                    if (j > i + 1)
                    {
                        if (norValue == null)
                        {
                            norValue = new StringBuilder(len);
                        }
                        norValue.Append(value, startPos, i - startPos + 1);
                        startPos = j;
                        i = j - 1;
                    }
                }
            }
            if (norValue == null)
            {
                return (startPos == 0) ? value : value.Substring(startPos, len - startPos);
            }
            else
            {
                if (i > startPos)
                {
                    norValue.Append(value, startPos, i - startPos);
                }
                return norValue.ToString();
            }
        }

        /// <summary>
        /// StripSpaces removes spaces at the beginning and at the end of the
        /// value and replaces sequences of spaces with a single space.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        /// <param name="len">The length.</param>
        internal static void StripSpaces(char[] value, int index, ref int len)
        {
            if (len <= 0)
            {
                return;
            }

            int startPos = index;
            int endPos = index + len;

            while (value[startPos] == 0x20)
            {
                startPos++;
                if (startPos == endPos)
                {
                    len = 1;
                    return;
                }
            }

            int offset = startPos - index;
            int i;
            for (i = startPos; i < endPos; i++)
            {
                char ch;
                if ((ch = value[i]) == 0x20)
                {
                    int j = i + 1;
                    while (j < endPos && value[j] == 0x20)
                    {
                        j++;
                    }
                    if (j == endPos)
                    {
                        offset += (j - i);
                        break;
                    }
                    if (j > i + 1)
                    {
                        offset += (j - i - 1);
                        i = j - 1;
                    }
                }
                value[i - offset] = ch;
            }
            len -= offset;
        }

        internal static void BlockCopyChars(char[] src, int srcOffset, char[] dst, int dstOffset, int count)
        {
            // PERF: Buffer.BlockCopy is faster than Array.Copy
            Buffer.BlockCopy(src, srcOffset * sizeof(char), dst, dstOffset * sizeof(char), count * sizeof(char));
        }

        internal static void BlockCopy(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        {
            Buffer.BlockCopy(src, srcOffset, dst, dstOffset, count);
        }

        /// <summary>
        /// When overridden in a derived class, changes the
        /// <see cref="ReadState"/> to <see cref="ReadState.Closed"/>.
        /// </summary>
        public virtual void Close()
        {
            ReadState = ReadState.Closed;
            textReader.Close();
        }

        /// <summary>
        /// When overridden in a derived class, gets the value of the attribute
        /// with the specified <see cref="Name"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string GetAttribute(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, gets the value of the attribute
        /// with the specified index.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string GetAttribute(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously gets the value of the current node.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual Task<string> GetValueAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calls <see cref="MoveToContent()"/> and tests if the current
        /// content node is a start tag or empty element tag.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        ///   <c>true</c> if [is start element] [the specified name]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual bool IsStartElement(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calls <see cref="MoveToContent()"/> and tests if the current
        /// content node is a start tag or empty element tag and if the
        /// <see cref="Name"/> property of the element found matches the given
        /// argument.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is start element]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual bool IsStartElement()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, moves to the attribute with
        /// the specified index.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual bool MoveToAttribute(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, moves to the attribute with the
        /// specified Name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual bool MoveToAttribute(string name)
        {
            int i = GetIndexOfAttributeWithoutPrefix(name);

            if (i >= 0)
            {
                if (InAttributeValueIterator)
                {
                    FinishAttributeValueIterator();
                }

                _curAttrIndex = i - _index - 1;
                _curNode = _nodes[i];

                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual RazorNodeType MoveToContent()
        {
            throw new NotImplementedException();
        }

        public virtual Task<RazorNodeType> MoveToContentAsync()
        {
            throw new NotImplementedException();
        }

        public virtual bool MoveToElement()
        {
            throw new NotImplementedException();
        }

        public virtual bool MoveToFirstAttribute()
        {
            throw new NotImplementedException();
        }

        public virtual bool MoveToNextAttribute()
        {
            throw new NotImplementedException();
        }

        public virtual bool Read()
        {
            for (;;)
            {
                switch (_parsingFunction)
                {
                    case ParsingFunction.ElementContent:
                        return ParseElementContent();

                    case ParsingFunction.DocumentContent:
                        return ParseDocumentContent();

                    case ParsingFunction.SwitchToInteractiveXmlDecl:
                        _readState = ReadState.Interactive;
                        _parsingFunction = _nextParsingFunction;
                        _reportedEncoding = _ps.encoding;
                        continue;

                    case ParsingFunction.NoData:
                        return false;

                    case ParsingFunction.GoToEof:
                        OnEof();
                        return false;

                    case ParsingFunction.Error:
                    case ParsingFunction.Eof:
                    case ParsingFunction.ReaderClosed:
                        return false;

                    default:
                        Debug.Fail($"Unexpected parsing function {_parsingFunction}");
                        break;
                }
            }

            throw new NotImplementedException();
        }

        public virtual Task<bool> ReadAsync()
        {
            throw new NotImplementedException();
        }

        public virtual bool ReadAttributeValue()
        {
            throw new NotImplementedException();
        }

        public virtual string ReadElementContentAsString(string name)
        {
            throw new NotImplementedException();
        }

        public virtual string ReadElementContentAsString()
        {
            throw new NotImplementedException();
        }

        public virtual void ReadEndElement()
        {
            throw new NotImplementedException();
        }

        public virtual string ReadInner()
        {
            throw new NotImplementedException();
        }

        public virtual Task<string> ReadInnerAsync()
        {
            throw new NotImplementedException();
        }

        public virtual string ReadOuterXml()
        {
            throw new NotImplementedException();
        }

        public virtual Task<string> ReadOuterXmlAsync()
        {
            throw new NotImplementedException();
        }

        public virtual void ReadStartElement()
        {
            throw new NotImplementedException();
        }

        public virtual void ReadStartElement(string name)
        {
            throw new NotImplementedException();
        }

        public virtual XmlReader ReadSubtree()
        {
            throw new NotImplementedException();
        }

        public virtual bool ReadToDescendant(string name)
        {
            throw new NotImplementedException();
        }

        public virtual bool ReadToFollowing(string name)
        {
            throw new NotImplementedException();
        }

        public virtual bool ReadToNextSibling(string name)
        {
            throw new NotImplementedException();
        }

        public virtual int ReadValueChunk(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public virtual Task<int> ReadValueChunkAsync(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public virtual void Skip()
        {
            throw new NotImplementedException();
        }

        public virtual Task SkipAsync()
        {
            throw new NotImplementedException();
        }

        // Reads more data to the character buffer, discarding already parsed chars / decoded bytes.
        private int ReadData()
        {
            // Append Mode:  Append new bytes and characters to the buffers, do not rewrite them. Allocate new buffers
            //               if the current ones are full
            // Rewrite Mode: Reuse the buffers. If there is less than half of the char buffer left for new data, move 
            //               the characters that has not been parsed yet to the front of the buffer. Same for bytes.

            if (_ps.isEof)
            {
                return 0;
            }

            int charsRead;
            if (_ps.appendMode)
            {
                // the character buffer is full -> allocate a new one
                if (_ps.charsUsed == _ps.chars.Length - 1)
                {
                    // invalidate node values kept in buffer - applies to attribute values only
                    for (int i = 0; i < _attrCount; i++)
                    {
                        _nodes[_index + i + 1].OnBufferInvalidated();
                    }

                    char[] newChars = new char[_ps.chars.Length * 2];
                    BlockCopyChars(_ps.chars, 0, newChars, 0, _ps.chars.Length);
                    _ps.chars = newChars;
                }

                if (_ps.stream != null)
                {
                    // the byte buffer is full -> allocate a new one
                    if (_ps.bytesUsed - _ps.bytePos < MaxByteSequenceLen)
                    {
                        if (_ps.bytes.Length - _ps.bytesUsed < MaxByteSequenceLen)
                        {
                            byte[] newBytes = new byte[_ps.bytes.Length * 2];
                            BlockCopy(_ps.bytes, 0, newBytes, 0, _ps.bytesUsed);
                            _ps.bytes = newBytes;
                        }
                    }
                }

                charsRead = _ps.chars.Length - _ps.charsUsed - 1;
                if (charsRead > ApproxXmlDeclLength)
                {
                    charsRead = ApproxXmlDeclLength;
                }
            }
            else
            {
                int charsLen = _ps.chars.Length;
                if (charsLen - _ps.charsUsed <= charsLen / 2)
                {
                    // invalidate node values kept in buffer - applies to attribute values only
                    for (int i = 0; i < _attrCount; i++)
                    {
                        _nodes[_index + i + 1].OnBufferInvalidated();
                    }

                    // move unparsed characters to front, unless the whole buffer contains unparsed characters
                    int copyCharsCount = _ps.charsUsed - _ps.charPos;
                    if (copyCharsCount < charsLen - 1)
                    {
                        _ps.lineStartPos = _ps.lineStartPos - _ps.charPos;
                        if (copyCharsCount > 0)
                        {
                            BlockCopyChars(_ps.chars, _ps.charPos, _ps.chars, 0, copyCharsCount);
                        }
                        _ps.charPos = 0;
                        _ps.charsUsed = copyCharsCount;
                    }
                    else
                    {
                        char[] newChars = new char[_ps.chars.Length * 2];
                        BlockCopyChars(_ps.chars, 0, newChars, 0, _ps.chars.Length);
                        _ps.chars = newChars;
                    }
                }

                if (_ps.stream != null)
                {
                    // move undecoded bytes to the front to make some space in the byte buffer
                    int bytesLeft = _ps.bytesUsed - _ps.bytePos;
                    if (bytesLeft <= MaxBytesToMove)
                    {
                        if (bytesLeft == 0)
                        {
                            _ps.bytesUsed = 0;
                        }
                        else
                        {
                            BlockCopy(_ps.bytes, _ps.bytePos, _ps.bytes, 0, bytesLeft);
                            _ps.bytesUsed = bytesLeft;
                        }
                        _ps.bytePos = 0;
                    }
                }
                charsRead = _ps.chars.Length - _ps.charsUsed - 1;
            }

            if (_ps.stream != null)
            {
                if (!_ps.isStreamEof)
                {
                    // read new bytes
                    if (_ps.bytePos == _ps.bytesUsed && _ps.bytes.Length - _ps.bytesUsed > 0)
                    {
                        int read = _ps.stream.Read(_ps.bytes, _ps.bytesUsed, _ps.bytes.Length - _ps.bytesUsed);
                        if (read == 0)
                        {
                            _ps.isStreamEof = true;
                        }
                        _ps.bytesUsed += read;
                    }
                }

                int originalBytePos = _ps.bytePos;

                // decode chars
                charsRead = GetChars(charsRead);
                if (charsRead == 0 && _ps.bytePos != originalBytePos)
                {
                    // GetChars consumed some bytes but it was not enough bytes to form a character -> try again
                    return ReadData();
                }
            }
            else if (_ps.textReader != null)
            {
                // read chars
                charsRead = _ps.textReader.Read(_ps.chars, _ps.charsUsed, _ps.chars.Length - _ps.charsUsed - 1);
                _ps.charsUsed += charsRead;
            }
            else
            {
                charsRead = 0;
            }

            RegisterConsumedCharacters(charsRead, InEntity);

            if (charsRead == 0)
            {
                Debug.Assert(_ps.charsUsed < _ps.chars.Length);
                _ps.isEof = true;
            }
            _ps.chars[_ps.charsUsed] = (char)0;
            return charsRead;
        }

        // Stream input only: read bytes from stream and decodes them according to the current encoding 
        private int GetChars(int maxCharsCount)
        {
            Debug.Assert(_ps.stream != null
                && _ps.decoder != null
                && _ps.bytes != null);
            Debug.Assert(maxCharsCount <= _ps.chars.Length - _ps.charsUsed - 1);

            // determine the maximum number of bytes we can pass to the decoder
            int bytesCount = _ps.bytesUsed - _ps.bytePos;
            if (bytesCount == 0)
            {
                return 0;
            }

            int charsCount;
            bool completed;
            try
            {
                // decode chars
                _ps.decoder.Convert(_ps.bytes, _ps.bytePos, bytesCount, _ps.chars, _ps.charsUsed, maxCharsCount, false, out bytesCount, out charsCount, out completed);
            }
            catch (ArgumentException)
            {
                InvalidCharRecovery(ref bytesCount, out charsCount);
            }

            // move pointers and return
            _ps.bytePos += bytesCount;
            _ps.charsUsed += charsCount;
            Debug.Assert(maxCharsCount >= charsCount);
            return charsCount;
        }

        private void InvalidCharRecovery(ref int bytesCount, out int charsCount)
        {
            int charsDecoded = 0;
            int bytesDecoded = 0;
            try
            {
                while (bytesDecoded < bytesCount)
                {
                    _ps.decoder.Convert(
                        bytes: _ps.bytes,
                        byteIndex: _ps.bytePos + bytesDecoded,
                        byteCount: 1,
                        chars: _ps.chars,
                        charIndex: _ps.charsUsed + charsDecoded,
                        charCount: 2,
                        flush: false,
                        bytesUsed: out int bDec,
                        charsUsed: out int chDec,
                        completed: out bool completed);

                    charsDecoded += chDec;
                    bytesDecoded += bDec;
                }
                Debug.Fail("We should get an exception again.");
            }
            catch (ArgumentException)
            {
            }

            if (charsDecoded == 0)
            {
                Throw(_ps.charsUsed, SR.Xml_InvalidCharInThisEncoding);
            }
            charsCount = charsDecoded;
            bytesCount = bytesDecoded;
        }

        private int GetIndexOfAttributeWithoutPrefix(string name)
        {
            name = _nameTable.Get(name);
            if (name == null)
            {
                return -1;
            }
            for (int i = _index + 1; i < _index + _attrCount + 1; i++)
            {
                if (Ref.Equal(_nodes[i].localName, name) && _nodes[i].prefix.Length == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private void FinishAttributeValueIterator()
        {
            Debug.Assert(InAttributeValueIterator);
            if (_parsingFunction == ParsingFunction.InReadValueChunk)
            {
                FinishReadValueChunk();
            }
            else if (_parsingFunction == ParsingFunction.InReadContentAsBinary)
            {
                FinishReadContentAsBinary();
            }
            if (_parsingFunction == ParsingFunction.InReadAttributeValue)
            {
                while (_ps.entityId != _attributeValueBaseEntityId)
                {
                    HandleEntityEnd(false);
                }
                _emptyEntityInAttributeResolved = false;
                _parsingFunction = _nextParsingFunction;
                _nextParsingFunction = (_index > 0) ? ParsingFunction.ElementContent : ParsingFunction.DocumentContent;
            }
        }

        private void FinishReadValueChunk()
        {
            Debug.Assert(_parsingFunction == ParsingFunction.InReadValueChunk);

            _readValueOffset = 0;
            if (_incReadState == IncrementalReadState.ReadValueChunk_OnPartialValue)
            {
                Debug.Assert((_index > 0) ? _nextParsingFunction == ParsingFunction.ElementContent : _nextParsingFunction == ParsingFunction.DocumentContent);
                SkipPartialTextValue();
            }
            else
            {
                _parsingFunction = _nextParsingFunction;
                _nextParsingFunction = _nextNextParsingFunction;
            }
        }

        private bool IsTextValidStartTag()
        {
            var nextLetter = (char)textReader.Peek();

            return nextLetter.Equals('@') || nextLetter.Equals('<');
        }

        private bool IsTextValidEndTag()
        {
            var nextLetter = (char)textReader.Peek();

            return nextLetter.Equals(' ') || nextLetter.Equals('>');
        }

        private void OnEof()
        {
            Debug.Assert(_ps.isEof);
            _curNode = _nodes[0];
            _curNode.Clear(XmlNodeType.None);
            _curNode.SetLineInfo(_ps.LineNo, _ps.LinePos);

            _parsingFunction = ParsingFunction.Eof;
            _readState = ReadState.EndOfFile;

            _reportedEncoding = null;
        }

        private bool ParseElementContent()
        {
            for (;;)
            {
                int pos = _ps.charPos;
                char[] chars = _ps.chars;

                switch (chars[pos])
                {
                    // some tag
                    case '<':
                        switch (chars[pos + 1])
                        {
                            // processing instruction
                            case '?':
                                _ps.charPos = pos + 2;
                                if (ParsePI())
                                {
                                    return true;
                                }
                                continue;
                            case '!':
                                pos += 2;
                                if (_ps.charsUsed - pos < 2)
                                    goto ReadData;
                                // comment
                                if (chars[pos] == '-')
                                {
                                    if (chars[pos + 1] == '-')
                                    {
                                        _ps.charPos = pos + 2;
                                        if (ParseComment())
                                        {
                                            return true;
                                        }
                                        continue;
                                    }
                                    else
                                    {
                                        ThrowUnexpectedToken(pos + 1, "-");
                                    }
                                }
                                // CDATA section
                                else if (chars[pos] == '[')
                                {
                                    pos++;
                                    if (_ps.charsUsed - pos < 6)
                                    {
                                        goto ReadData;
                                    }
                                    if (XmlConvert.StrEqual(chars, pos, 6, "CDATA["))
                                    {
                                        _ps.charPos = pos + 6;
                                        ParseCData();
                                        return true;
                                    }
                                    else
                                    {
                                        ThrowUnexpectedToken(pos, "CDATA[");
                                    }
                                }
                                else
                                {
                                    if (ParseUnexpectedToken(pos) == "DOCTYPE")
                                    {
                                        Throw(SR.Xml_BadDTDLocation);
                                    }
                                    else
                                    {
                                        ThrowUnexpectedToken(pos, "<!--", "<[CDATA[");
                                    }
                                }
                                break;
                            // element end tag
                            case '/':
                                _ps.charPos = pos + 2;
                                ParseEndElement();
                                return true;
                            default:
                                // end of buffer
                                if (pos + 1 == _ps.charsUsed)
                                {
                                    goto ReadData;
                                }
                                else
                                {
                                    // element start tag
                                    _ps.charPos = pos + 1;
                                    ParseElement();
                                    return true;
                                }
                        }
                        break;
                    case '&':
                        if (ParseText())
                        {
                            return true;
                        }
                        continue;
                    default:
                        // end of buffer
                        if (pos == _ps.charsUsed)
                        {
                            goto ReadData;
                        }
                        else
                        {
                            // text node, whitespace or entity reference
                            if (ParseText())
                            {
                                return true;
                            }
                            continue;
                        }
                }

                ReadData:
                // read new characters into the buffer
                if (ReadData() == 0)
                {
                    if (_ps.charsUsed - _ps.charPos != 0)
                    {
                        ThrowUnclosedElements();
                    }
                    if (!InEntity)
                    {
                        if (_index == 0 && _fragmentType != XmlNodeType.Document)
                        {
                            OnEof();
                            return false;
                        }
                        ThrowUnclosedElements();
                    }
                    if (HandleEntityEnd(true))
                    {
                        SetupEndEntityNodeInContent();
                        return true;
                    }
                }
            }
        }

        private bool ParseComment()
        {
            if (_ignoreComments)
            {
                ParsingMode oldParsingMode = _parsingMode;
                _parsingMode = ParsingMode.SkipNode;
                ParseCDataOrComment(RazorNodeType.Comment);
                _parsingMode = oldParsingMode;
                return false;
            }
            else
            {
                ParseCDataOrComment(RazorNodeType.Comment);
                return true;
            }
        }

        // Parses CDATA section or comment
        private void ParseCDataOrComment(RazorNodeType type)
        {
            int startPos, endPos;

            if (_parsingMode == ParsingMode.Full)
            {
                _curNode.SetLineInfo(_ps.LineNo, _ps.LinePos);
                Debug.Assert(_stringBuilder.Length == 0);
                if (ParseCDataOrComment(type, out startPos, out endPos))
                {
                    _curNode.SetValueNode(type, _ps.chars, startPos, endPos - startPos);
                }
                else
                {
                    do
                    {
                        _stringBuilder.Append(_ps.chars, startPos, endPos - startPos);
                    } while (!ParseCDataOrComment(type, out startPos, out endPos));
                    _stringBuilder.Append(_ps.chars, startPos, endPos - startPos);
                    _curNode.SetValueNode(type, _stringBuilder.ToString());
                    _stringBuilder.Length = 0;
                }
            }
            else
            {
                while (!ParseCDataOrComment(type, out startPos, out endPos)) ;
            }
        }

        // Parses a chunk of CDATA section or comment. Returns true when the end of CDATA or comment was reached.
        private bool ParseCDataOrComment(XmlNodeType type, out int outStartPos, out int outEndPos)
        {
            if (_ps.charsUsed - _ps.charPos < 3)
            {
                // read new characters into the buffer
                if (ReadData() == 0)
                {
                    Throw(SR.Xml_UnexpectedEOF, (type == XmlNodeType.Comment) ? "Comment" : "CDATA");
                }
            }

            int pos = _ps.charPos;
            char[] chars = _ps.chars;
            int rcount = 0;
            int rpos = -1;
            char stopChar = (type == XmlNodeType.Comment) ? '-' : ']';

            for (; ; )
            {
                char tmpch;
                unsafe
                {
                    while (_xmlCharType.IsTextChar(tmpch = chars[pos]) && tmpch != stopChar)
                    {
                        pos++;
                    }
                }

                // possibly end of comment or cdata section
                if (chars[pos] == stopChar)
                {
                    if (chars[pos + 1] == stopChar)
                    {
                        if (chars[pos + 2] == '>')
                        {
                            if (rcount > 0)
                            {
                                Debug.Assert(!_ps.eolNormalized);
                                ShiftBuffer(rpos + rcount, rpos, pos - rpos - rcount);
                                outEndPos = pos - rcount;
                            }
                            else
                            {
                                outEndPos = pos;
                            }
                            outStartPos = _ps.charPos;
                            _ps.charPos = pos + 3;
                            return true;
                        }
                        else if (pos + 2 == _ps.charsUsed)
                        {
                            goto ReturnPartial;
                        }
                        else if (type == XmlNodeType.Comment)
                        {
                            Throw(pos, SR.Xml_InvalidCommentChars);
                        }
                    }
                    else if (pos + 1 == _ps.charsUsed)
                    {
                        goto ReturnPartial;
                    }
                    pos++;
                    continue;
                }
                else
                {
                    switch (chars[pos])
                    {
                        // eol
                        case (char)0xA:
                            pos++;
                            OnNewLine(pos);
                            continue;
                        case (char)0xD:
                            if (chars[pos + 1] == (char)0xA)
                            {
                                // EOL normalization of 0xD 0xA - shift the buffer
                                if (!_ps.eolNormalized && _parsingMode == ParsingMode.Full)
                                {
                                    if (pos - _ps.charPos > 0)
                                    {
                                        if (rcount == 0)
                                        {
                                            rcount = 1;
                                            rpos = pos;
                                        }
                                        else
                                        {
                                            ShiftBuffer(rpos + rcount, rpos, pos - rpos - rcount);
                                            rpos = pos - rcount;
                                            rcount++;
                                        }
                                    }
                                    else
                                    {
                                        _ps.charPos++;
                                    }
                                }
                                pos += 2;
                            }
                            else if (pos + 1 < _ps.charsUsed || _ps.isEof)
                            {
                                if (!_ps.eolNormalized)
                                {
                                    chars[pos] = (char)0xA;             // EOL normalization of 0xD
                                }
                                pos++;
                            }
                            else
                            {
                                goto ReturnPartial;
                            }
                            OnNewLine(pos);
                            continue;
                        case '<':
                        case '&':
                        case ']':
                        case (char)0x9:
                            pos++;
                            continue;
                        default:
                            // end of buffer
                            if (pos == _ps.charsUsed)
                            {
                                goto ReturnPartial;
                            }
                            // surrogate characters
                            char ch = chars[pos];
                            if (XmlCharType.IsHighSurrogate(ch))
                            {
                                if (pos + 1 == _ps.charsUsed)
                                {
                                    goto ReturnPartial;
                                }
                                pos++;
                                if (XmlCharType.IsLowSurrogate(chars[pos]))
                                {
                                    pos++;
                                    continue;
                                }
                            }
                            ThrowInvalidChar(chars, _ps.charsUsed, pos);
                            break;
                    }
                }

                ReturnPartial:
                if (rcount > 0)
                {
                    ShiftBuffer(rpos + rcount, rpos, pos - rpos - rcount);
                    outEndPos = pos - rcount;
                }
                else
                {
                    outEndPos = pos;
                }
                outStartPos = _ps.charPos;

                _ps.charPos = pos;
                return false; // false == parsing of comment or CData section is not finished yet, must be called again
            }
        }

        // Parses the document content
        private bool ParseDocumentContent()
        {
            bool mangoQuirks = false;
#if FEATURE_LEGACYNETCF
            // In Mango the default XmlTextReader is instantiated
            // with v1Compat flag set to true.  One of the effects
            // of this settings is to eat any trailing nulls in the
            // buffer and some apps depend on this behavior.
            if (CompatibilitySwitches.IsAppEarlierThanWindowsPhone8)
                mangoQuirks = true;
#endif
            for (;;)
            {
                bool needMoreChars = false;
                int pos = _ps.charPos;
                char[] chars = _ps.chars;

                // some tag
                if (chars[pos] == '<')
                {
                    needMoreChars = true;
                    if (_ps.charsUsed - pos < 4) // minimum  "<a/>"
                        goto ReadData;
                    pos++;
                    switch (chars[pos])
                    {
                        // processing instruction
                        case '?':
                            _ps.charPos = pos + 1;
                            if (ParsePI())
                            {
                                return true;
                            }
                            continue;
                        case '!':
                            pos++;
                            if (_ps.charsUsed - pos < 2) // minimum characters expected "--"
                                goto ReadData;
                            // comment
                            if (chars[pos] == '-')
                            {
                                if (chars[pos + 1] == '-')
                                {
                                    _ps.charPos = pos + 2;
                                    if (ParseComment())
                                    {
                                        return true;
                                    }
                                    continue;
                                }
                                else
                                {
                                    ThrowUnexpectedToken(pos + 1, "-");
                                }
                            }
                            // CDATA section
                            else if (chars[pos] == '[')
                            {
                                if (_fragmentType != XmlNodeType.Document)
                                {
                                    pos++;
                                    if (_ps.charsUsed - pos < 6)
                                    {
                                        goto ReadData;
                                    }
                                    if (XmlConvert.StrEqual(chars, pos, 6, "CDATA["))
                                    {
                                        _ps.charPos = pos + 6;
                                        ParseCData();
                                        if (_fragmentType == XmlNodeType.None)
                                        {
                                            _fragmentType = XmlNodeType.Element;
                                        }
                                        return true;
                                    }
                                    else
                                    {
                                        ThrowUnexpectedToken(pos, "CDATA[");
                                    }
                                }
                                else
                                {
                                    Throw(_ps.charPos, SR.Xml_InvalidRootData);
                                }
                            }
                            // DOCTYPE declaration
                            else
                            {
                                if (_fragmentType == XmlNodeType.Document || _fragmentType == XmlNodeType.None)
                                {
                                    _fragmentType = XmlNodeType.Document;
                                    _ps.charPos = pos;
                                    if (ParseDoctypeDecl())
                                    {
                                        return true;
                                    }
                                    continue;
                                }
                                else
                                {
                                    if (ParseUnexpectedToken(pos) == "DOCTYPE")
                                    {
                                        Throw(SR.Xml_BadDTDLocation);
                                    }
                                    else
                                    {
                                        ThrowUnexpectedToken(pos, "<!--", "<[CDATA[");
                                    }
                                }
                            }
                            break;
                        case '/':
                            Throw(pos + 1, SR.Xml_UnexpectedEndTag);
                            break;
                        // document element start tag
                        default:
                            if (_rootElementParsed)
                            {
                                if (_fragmentType == RazorNodeType.Document)
                                {
                                    Throw(pos, SR.Xml_MultipleRoots);
                                }
                                if (_fragmentType == RazorNodeType.None)
                                {
                                    _fragmentType = RazorNodeType.Element;
                                }
                            }
                            _ps.charPos = pos;
                            _rootElementParsed = true;
                            ParseElement();
                            return true;
                    }
                }
                else if (chars[pos] == '&')
                {
                    if (_fragmentType == RazorNodeType.Document)
                    {
                        Throw(pos, SR.Xml_InvalidRootData);
                    }
                    else
                    {
                        if (_fragmentType == RazorNodeType.None)
                        {
                            _fragmentType = RazorNodeType.Element;
                        }
                        int i;
                        switch (HandleEntityReference(false, EntityExpandType.OnlyGeneral, out i))
                        {
                            case EntityType.Unexpanded:
                                if (_parsingFunction == ParsingFunction.EntityReference)
                                {
                                    _parsingFunction = _nextParsingFunction;
                                }
                                ParseEntityReference();
                                return true;
                            case EntityType.CharacterDec:
                            case EntityType.CharacterHex:
                            case EntityType.CharacterNamed:
                                if (ParseText())
                                {
                                    return true;
                                }
                                continue;
                            default:
                                chars = _ps.chars;
                                pos = _ps.charPos;
                                continue;
                        }
                    }
                }
                // end of buffer
                else if (pos == _ps.charsUsed || ((_v1Compat || mangoQuirks) && chars[pos] == 0x0))
                {
                    goto ReadData;
                }
                // something else -> root level whitespace
                else
                {
                    if (_fragmentType == RazorNodeType.Document)
                    {
                        if (ParseRootLevelWhitespace())
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (ParseText())
                        {
                            if (_fragmentType == RazorNodeType.None && _curNode.type == RazorNodeType.Text)
                            {
                                _fragmentType = RazorNodeType.Element;
                            }
                            return true;
                        }
                    }
                    continue;
                }

                Debug.Assert(pos == _ps.charsUsed && !_ps.isEof);

                ReadData:
                // read new characters into the buffer
                if (ReadData() != 0)
                {
                    pos = _ps.charPos;
                }
                else
                {
                    if (needMoreChars)
                    {
                        Throw(SR.Xml_InvalidRootData);
                    }

                    if (InEntity)
                    {
                        if (HandleEntityEnd(true))
                        {
                            SetupEndEntityNodeInContent();
                            return true;
                        }
                        continue;
                    }
                    Debug.Assert(_index == 0);

                    if (!_rootElementParsed && _fragmentType == RazorNodeType.Document)
                    {
                        ThrowWithoutLineInfo(SR.Xml_MissingRoot);
                    }

                    if (_fragmentType == RazorNodeType.None)
                    {
                        _fragmentType = _rootElementParsed
                            ? RazorNodeType.Document
                            : RazorNodeType.Element;
                    }
                    OnEof();
                    return false;
                }

                pos = _ps.charPos;
                chars = _ps.chars;
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    textReader?.Dispose();
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RazorReader() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        public bool HasLineInfo()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private helper types

        internal struct LineInfo
        {
            internal int lineNo;
            internal int linePos;

            public LineInfo(int lineNo, int linePos)
            {
                this.lineNo = lineNo;
                this.linePos = linePos;
            }

            public void Set(int lineNo, int linePos)
            {
                this.lineNo = lineNo;
                this.linePos = linePos;
            }
        }

        //
        // ParsingState
        //
        // Parsing state (aka. scanner data) - holds parsing buffer and entity input data information
        private struct ParsingState
        {
            // character buffer
            internal char[] chars;
            internal int charPos;
            internal int charsUsed;
            internal Encoding encoding;
            internal bool appendMode;

            // input stream & byte buffer
            internal Stream stream;
            internal Decoder decoder;
            internal byte[] bytes;
            internal int bytePos;
            internal int bytesUsed;

            // input text reader
            internal TextReader textReader;

            // current line number & position
            internal int lineNo;
            internal int lineStartPos;

            // base uri of the current entity
            internal string baseUriStr;
            internal Uri baseUri;

            // eof flag of the entity
            internal bool isEof;
            internal bool isStreamEof;

            // entity type & id
            internal IDtdEntityInfo entity;
            internal int entityId;

            // normalization
            internal bool eolNormalized;

            // EndEntity reporting
            internal bool entityResolvedManually;

            internal void Clear()
            {
                chars = null;
                charPos = 0;
                charsUsed = 0;
                encoding = null;
                stream = null;
                decoder = null;
                bytes = null;
                bytePos = 0;
                bytesUsed = 0;
                textReader = null;
                lineNo = 1;
                lineStartPos = -1;
                baseUriStr = string.Empty;
                baseUri = null;
                isEof = false;
                isStreamEof = false;
                eolNormalized = true;
                entityResolvedManually = false;
            }

            internal void Close(bool closeInput)
            {
                if (closeInput)
                {
                    if (stream != null)
                    {
                        stream.Dispose();
                    }
                    else if (textReader != null)
                    {
                        textReader.Dispose();
                    }
                }
            }

            internal int LineNo
            {
                get
                {
                    return lineNo;
                }
            }

            internal int LinePos
            {
                get
                {
                    return charPos - lineStartPos;
                }
            }
        }

        private class NodeData : IComparable
        {
            // static instance with no data - is used when XmlTextReader is closed
            private static volatile NodeData s_None;

            // NOTE: Do not use this property for reference comparison. It may not be unique.
            internal static NodeData None
            {
                get
                {
                    if (s_None == null)
                    {
                        // no locking; s_None is immutable so it's not a problem that it may get initialized more than once
                        s_None = new NodeData();
                    }
                    return s_None;
                }
            }

            // type
            internal XmlNodeType type;

            // name
            internal string localName;
            internal string prefix;
            internal string ns;
            internal string nameWPrefix;

            // value:
            // value == null -> the value is kept in the 'chars' buffer starting at valueStartPos and valueLength long
            private string _value;
            private char[] _chars;
            private int _valueStartPos;
            private int _valueLength;

            // main line info
            internal LineInfo lineInfo;

            // second line info
            internal LineInfo lineInfo2;

            // quote char for attributes
            internal char quoteChar;

            // depth
            internal int depth;

            // empty element / default attribute
            private bool _isEmptyOrDefault;

            // entity id
            internal int entityId;

            // helper members
            internal bool xmlContextPushed;

            // attribute value chunks
            internal NodeData nextAttrValueChunk;

            // type info
            internal object schemaType;
            internal object typedValue;

            internal NodeData()
            {
                Clear(XmlNodeType.None);
                xmlContextPushed = false;
            }

            internal int LineNo
            {
                get
                {
                    return lineInfo.lineNo;
                }
            }

            internal int LinePos
            {
                get
                {
                    return lineInfo.linePos;
                }
            }

            internal bool IsEmptyElement
            {
                get
                {
                    return type == XmlNodeType.Element && _isEmptyOrDefault;
                }
                set
                {
                    Debug.Assert(type == XmlNodeType.Element);
                    _isEmptyOrDefault = value;
                }
            }

            internal bool IsDefaultAttribute
            {
                get
                {
                    return type == XmlNodeType.Attribute && _isEmptyOrDefault;
                }
                set
                {
                    Debug.Assert(type == XmlNodeType.Attribute);
                    _isEmptyOrDefault = value;
                }
            }

            internal bool ValueBuffered
            {
                get
                {
                    return _value == null;
                }
            }

            internal string StringValue
            {
                get
                {
                    Debug.Assert(_valueStartPos >= 0 || _value != null, "Value not ready.");

                    if (_value == null)
                    {
                        _value = new string(_chars, _valueStartPos, _valueLength);
                    }
                    return _value;
                }
            }

            internal void TrimSpacesInValue()
            {
                if (ValueBuffered)
                {
                    RazorReader.StripSpaces(_chars, _valueStartPos, ref _valueLength);
                }
                else
                {
                    _value = XmlTextReaderImpl.StripSpaces(_value);
                }
            }

            internal void Clear(XmlNodeType type)
            {
                this.type = type;
                ClearName();
                _value = string.Empty;
                _valueStartPos = -1;
                nameWPrefix = string.Empty;
                schemaType = null;
                typedValue = null;
            }

            internal void ClearName()
            {
                localName = string.Empty;
                prefix = string.Empty;
                ns = string.Empty;
                nameWPrefix = string.Empty;
            }

            internal void SetLineInfo(int lineNo, int linePos)
            {
                lineInfo.Set(lineNo, linePos);
            }

            internal void SetLineInfo2(int lineNo, int linePos)
            {
                lineInfo2.Set(lineNo, linePos);
            }

            internal void SetValueNode(RazorNodeType type, string value)
            {
                Debug.Assert(value != null);

                this.type = type;
                ClearName();
                _value = value;
                _valueStartPos = -1;
            }

            internal void SetValueNode(RazorNodeType type, char[] chars, int startPos, int len)
            {
                this.type = type;
                ClearName();

                _value = null;
                _chars = chars;
                _valueStartPos = startPos;
                _valueLength = len;
            }

            internal void SetNamedNode(RazorNodeType type, string localName)
            {
                SetNamedNode(type, localName, string.Empty, localName);
            }

            internal void SetNamedNode(RazorNodeType type, string localName, string prefix, string nameWPrefix)
            {
                Debug.Assert(localName != null);
                Debug.Assert(localName.Length > 0);

                this.type = type;
                this.localName = localName;
                this.prefix = prefix;
                this.nameWPrefix = nameWPrefix;
                this.ns = string.Empty;
                _value = string.Empty;
                _valueStartPos = -1;
            }

            internal void SetValue(string value)
            {
                _valueStartPos = -1;
                _value = value;
            }

            internal void SetValue(char[] chars, int startPos, int len)
            {
                _value = null;
                _chars = chars;
                _valueStartPos = startPos;
                _valueLength = len;
            }

            internal void OnBufferInvalidated()
            {
                if (_value == null)
                {
                    Debug.Assert(_valueStartPos != -1);
                    Debug.Assert(_chars != null);
                    _value = new string(_chars, _valueStartPos, _valueLength);
                }
                _valueStartPos = -1;
            }

            internal void CopyTo(int valueOffset, StringBuilder sb)
            {
                if (_value == null)
                {
                    Debug.Assert(_valueStartPos != -1);
                    Debug.Assert(_chars != null);
                    sb.Append(_chars, _valueStartPos + valueOffset, _valueLength - valueOffset);
                }
                else
                {
                    if (valueOffset <= 0)
                    {
                        sb.Append(_value);
                    }
                    else
                    {
                        sb.Append(_value, valueOffset, _value.Length - valueOffset);
                    }
                }
            }

            internal int CopyTo(int valueOffset, char[] buffer, int offset, int length)
            {
                if (_value == null)
                {
                    Debug.Assert(_valueStartPos != -1);
                    Debug.Assert(_chars != null);
                    int copyCount = _valueLength - valueOffset;
                    if (copyCount > length)
                    {
                        copyCount = length;
                    }
                    XmlTextReaderImpl.BlockCopyChars(_chars, _valueStartPos + valueOffset, buffer, offset, copyCount);
                    return copyCount;
                }
                else
                {
                    int copyCount = _value.Length - valueOffset;
                    if (copyCount > length)
                    {
                        copyCount = length;
                    }
                    _value.CopyTo(valueOffset, buffer, offset, copyCount);
                    return copyCount;
                }
            }

            internal int CopyToBinary(IncrementalReadDecoder decoder, int valueOffset)
            {
                if (_value == null)
                {
                    Debug.Assert(_valueStartPos != -1);
                    Debug.Assert(_chars != null);
                    return decoder.Decode(_chars, _valueStartPos + valueOffset, _valueLength - valueOffset);
                }
                else
                {
                    return decoder.Decode(_value, valueOffset, _value.Length - valueOffset);
                }
            }

            internal void AdjustLineInfo(int valueOffset, bool isNormalized, ref LineInfo lineInfo)
            {
                if (valueOffset == 0)
                {
                    return;
                }
                if (_valueStartPos != -1)
                {
                    XmlTextReaderImpl.AdjustLineInfo(_chars, _valueStartPos, _valueStartPos + valueOffset, isNormalized, ref lineInfo);
                }
                else
                {
                    XmlTextReaderImpl.AdjustLineInfo(_value, 0, valueOffset, isNormalized, ref lineInfo);
                }
            }

            // This should be inlined by JIT compiler
            internal string GetNameWPrefix(XmlNameTable nt)
            {
                if (nameWPrefix != null)
                {
                    return nameWPrefix;
                }
                else
                {
                    return CreateNameWPrefix(nt);
                }
            }

            internal string CreateNameWPrefix(XmlNameTable nt)
            {
                Debug.Assert(nameWPrefix == null);
                if (prefix.Length == 0)
                {
                    nameWPrefix = localName;
                }
                else
                {
                    nameWPrefix = nt.Add(string.Concat(prefix, ":", localName));
                }
                return nameWPrefix;
            }

            int IComparable.CompareTo(object obj)
            {
                NodeData other = obj as NodeData;
                if (other != null)
                {
                    if (Ref.Equal(localName, other.localName))
                    {
                        if (Ref.Equal(ns, other.ns))
                        {
                            return 0;
                        }
                        else
                        {
                            return string.CompareOrdinal(ns, other.ns);
                        }
                    }
                    else
                    {
                        return string.CompareOrdinal(localName, other.localName);
                    }
                }
                else
                {
                    Debug.Fail("We should never get to this point.");
                    // 'other' is null, 'this' is not null. Always return 1, like "".CompareTo(null).
                    return 1;
                }
            }
        }

        //
        // Private helper types
        //
        // ParsingFunction = what should the reader do when the next Read() is called
        private enum ParsingFunction
        {
            ElementContent = 0,
            NoData,
            OpenUrl,
            SwitchToInteractive,
            SwitchToInteractiveXmlDecl,
            DocumentContent,
            MoveToElementContent,
            PopElementContext,
            PopEmptyElementContext,
            ResetAttributesRootLevel,
            Error,
            Eof,
            ReaderClosed,
            EntityReference,
            InIncrementalRead,
            FragmentAttribute,
            ReportEndEntity,
            AfterResolveEntityInContent,
            AfterResolveEmptyEntityInContent,
            XmlDeclarationFragment,
            GoToEof,
            PartialTextValue,

            // these two states must be last; see InAttributeValueIterator property
            InReadAttributeValue,
            InReadValueChunk,
            InReadContentAsBinary,
            InReadElementContentAsBinary,
        }

        private enum IncrementalReadState
        {
            // Following values are used in ReadText, ReadBase64 and ReadBinHex (V1 streaming methods)
            Text,
            StartTag,
            PI,
            CDATA,
            Comment,
            Attributes,
            AttributeValue,
            ReadData,
            EndElement,
            End,

            // Following values are used in ReadTextChunk, ReadContentAsBase64 and ReadBinHexChunk (V2 streaming methods)
            ReadValueChunk_OnCachedValue,
            ReadValueChunk_OnPartialValue,

            ReadContentAsBinary_OnCachedValue,
            ReadContentAsBinary_OnPartialValue,
            ReadContentAsBinary_End,
        }

        private enum ParsingMode
        {
            Full,
            SkipNode,
            SkipContent,
        }

        private enum EntityType
        {
            CharacterDec,
            CharacterHex,
            CharacterNamed,
            Expanded,
            Skipped,
            FakeExpanded,
            Unexpanded,
            ExpandedInAttribute,
        }

        #endregion
    }
}
