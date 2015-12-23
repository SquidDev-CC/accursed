ffi = require('ffi')
ffi.cdef [[
// Basics definitions
typedef unsigned char xmlChar;

typedef enum {
	XML_ELEMENT_NODE= 1,
	XML_ATTRIBUTE_NODE= 2,
	XML_TEXT_NODE= 3,
	XML_CDATA_SECTION_NODE= 4,
	XML_ENTITY_REF_NODE= 5,
	XML_ENTITY_NODE= 6,
	XML_PI_NODE= 7,
	XML_COMMENT_NODE= 8,
	XML_DOCUMENT_NODE= 9,
	XML_DOCUMENT_TYPE_NODE= 10,
	XML_DOCUMENT_FRAG_NODE= 11,
	XML_NOTATION_NODE= 12,
	XML_HTML_DOCUMENT_NODE= 13,
	XML_DTD_NODE= 14,
	XML_ELEMENT_DECL= 15,
	XML_ATTRIBUTE_DECL= 16,
	XML_ENTITY_DECL= 17,
	XML_NAMESPACE_DECL= 18,
	XML_XINCLUDE_START= 19,
	XML_XINCLUDE_END= 20
} xmlElementType;
typedef xmlElementType xmlNsType;

// Structures
typedef struct _xmlNs xmlNs;
typedef xmlNs *xmlNsPtr;

struct _xmlNode {
	void           *_private; /* application data */
	xmlElementType   type; /* type number, must be second ! */
	const xmlChar   *name;      /* the name of the node, or the entity */
	struct _xmlNode *children; /* parent->childs link */
	struct _xmlNode *last; /* last child link */
	struct _xmlNode *parent; /* child->parent link */
	struct _xmlNode *next; /* next sibling link  */
	struct _xmlNode *prev; /* previous sibling link  */
	struct _xmlDoc  *doc; /* the containing document */

	/* End of common part */
	xmlNs           *ns;        /* pointer to the associated namespace */
	xmlChar         *content;   /* the content */
	struct _xmlAttr *properties;/* properties list */
	xmlNs           *nsDef;     /* namespace definitions on this node */
	void            *psvi; /* for type/PSVI informations */
	unsigned short   line; /* line number */
	unsigned short   extra; /* extra data for XPath/XSLT */
};

struct _xmlDoc {
	void           *_private; /* application data */
	xmlElementType  type;       /* XML_DOCUMENT_NODE, must be second ! */
	char           *name; /* name/filename/URI of the document */
	struct _xmlNode *children; /* the document tree */
	struct _xmlNode *last; /* last child link */
	struct _xmlNode *parent; /* child->parent link */
	struct _xmlNode *next; /* next sibling link  */
	struct _xmlNode *prev; /* previous sibling link  */
	struct _xmlDoc  *doc; /* autoreference to itself */
};


// Simple typedefs
typedef struct _xmlDoc xmlDoc;
typedef struct _xmlNode xmlNode;
typedef xmlNode *xmlNodePtr;
typedef xmlDoc *xmlDocPtr;
typedef xmlDocPtr htmlDocPtr;

// Public functions
htmlDocPtr htmlParseDoc(const xmlChar *cur, const char *encoding);
void xmlFreeDoc(xmlDocPtr cur);

xmlChar * xmlNodeGetContent(const xmlNode *cur);
xmlChar *	xmlGetProp(const xmlNode * node, const xmlChar * name);

// XPath
typedef struct _xmlXPathObject xmlXPathObject;
typedef struct _xmlXPathContext xmlXPathContext;
typedef xmlXPathContext *xmlXPathContextPtr;
typedef xmlXPathObject *xmlXPathObjectPtr;

struct _xmlXPathContext {
	xmlDocPtr doc; /* The current document */
	xmlNodePtr node; /* The current node */
};

struct _xmlNodeSet {
	int nodeNr; /* number of nodes in the set */
	int nodeMax; /* size of the array as allocated */
	xmlNodePtr *nodeTab; /* array of nodes in no particular order */
};
typedef struct _xmlNodeSet xmlNodeSet;
typedef xmlNodeSet *xmlNodeSetPtr;

typedef enum {
	XPATH_UNDEFINED = 0,
	XPATH_NODESET = 1,
	XPATH_BOOLEAN = 2,
	XPATH_NUMBER = 3,
	XPATH_STRING = 4,
	XPATH_POINT = 5,
	XPATH_RANGE = 6,
	XPATH_LOCATIONSET = 7,
	XPATH_USERS = 8,
	XPATH_XSLT_TREE = 9
} xmlXPathObjectType;

struct _xmlXPathObject {
	xmlXPathObjectType type;
	xmlNodeSetPtr nodesetval;
	int boolval;
	double floatval;
	xmlChar *stringval;
	void *user;
	int index;
	void *user2;
	int index2;
};

// XPath methods
xmlXPathContextPtr xmlXPathNewContext(xmlDocPtr doc);
void xmlXPathFreeContext(xmlXPathContextPtr ctxt);

xmlXPathObjectPtr xmlXPathEvalExpression(const xmlChar *str, xmlXPathContextPtr ctxt);
void xmlXPathFreeObject(xmlXPathObjectPtr obj);

// Error handling
typedef struct _xmlError xmlError;
typedef xmlError *xmlErrorPtr;
typedef enum {
	XML_ERR_NONE = 0,
	XML_ERR_WARNING = 1, /* A simple warning */
	XML_ERR_ERROR = 2,  /* A recoverable error */
	XML_ERR_FATAL = 3  /* A fatal error */
} xmlErrorLevel;
struct _xmlError {
	int  domain; /* What part of the library raised this error */
	int code; /* The error code, e.g. an xmlParserError */
	char       *message;/* human-readable informative error message */
	xmlErrorLevel level;/* how consequent is the error */
	char       *file; /* the filename */
	int  line; /* the line number if available */
	char       *str1; /* extra string information */
	char       *str2; /* extra string information */
	char       *str3; /* extra string information */
	int  int1; /* extra number information */
	int  int2; /* error column # or 0 if N/A (todo: rename field when we would brk ABI) */
	void       *ctxt;   /* the parser context if available */
	void       *node;   /* the node in the tree */
};
typedef void (*xmlGenericErrorFunc) (void *ctx, const char *msg);
typedef void (*xmlStructuredErrorFunc) (void *userData, xmlErrorPtr error);
void xmlSetGenericErrorFunc(void * ctx, xmlGenericErrorFunc handler);
void xmlSetStructuredErrorFunc(void * ctx, xmlStructuredErrorFunc handler);
]]

xml = ffi.load "xml2"
xml.xmlSetStructuredErrorFunc nil, ffi.cast("xmlStructuredErrorFunc", () -> nil)
xml.xmlSetGenericErrorFunc nil, ffi.cast("xmlGenericErrorFunc", () -> nil)

mapping = {
	"ELEMENT_NODE"
	"ATTRIBUTE_NODE"
	"TEXT_NODE"
	"CDATA_SECTION_NODE"
	"ENTITY_REF_NODE"
	"ENTITY_NODE"
	"PI_NODE"
	"COMMENT_NODE"
	"DOCUMENT_NODE"
	"DOCUMENT_TYPE_NODE"
	"DOCUMENT_FRAG_NODE"
	"NOTATION_NODE"
	"HTML_DOCUMENT_NODE"
	"DTD_NODE"
	"ELEMENT_DECL"
	"ATTRIBUTE_DECL"
	"ENTITY_DECL"
	"NAMESPACE_DECL"
	"XINCLUDE_START"
	"XINCLUDE_END"
}

dump = (node, indent = "") ->
	assert node

	name = "<none>"
	if node.name ~= nil
		name = ffi.string(node.name)
	print indent .. name .. " " .. mapping[tonumber(node.type)]\lower! .. " (" .. tonumber(node.type) .. ")"
	if node.type < 13
		print indent .. "\tText => " .. ffi.string(xml.xmlNodeGetContent(node))
		if node.content ~= nil
			print indent .. "\tContent => " ..  ffi.string(node.content)

	child = node.children
	while child ~= nil
		dump(child, indent .. "\t")
		child = child.next

loadHTML = (src) ->
	if src == nil then error("expected string, got nil", 2)
	ffi.gc(xml.htmlParseDoc(src, "UTF-8"), xml.xmlFreeDoc)

xpath = (doc) ->
	if doc == nil then error("expected doc, got nil", 2)
	selector = ffi.gc(xml.xmlXPathNewContext(doc), xml.xmlXPathFreeContext)

	(query, node) ->
		if query == nil then error("expected string, got nil", 2)
		selector.node = node
		expr = ffi.gc(xml.xmlXPathEvalExpression(query, selector), xml.xmlXPathFreeObject)
		set = expr.nodesetval

		[set.nodeTab[i] for i = 0, set.nodeNr - 1]

contents = (node) ->
	if node == nil then error("expected node, got nil", 2)
	content = xml.xmlNodeGetContent(node)
	if content == nil then nil else ffi.string(content)

attribute = (node, name) ->
	if node == nil then error("expected node, got nil", 2)
	if name == nil then error("expected string, got nil", 2)

	content = xml.xmlGetProp(node, name)
	if content == nil then nil else ffi.string(content)

{ :xml, :dump, :loadHTML, :xpath, :contents, :attribute }