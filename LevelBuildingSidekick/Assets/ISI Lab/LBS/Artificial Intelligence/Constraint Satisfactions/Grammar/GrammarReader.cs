using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Speech.Recognition.SrgsGrammar;
using System;
using System.IO;
using System.Xml;
using UnityEngine.UI;
using System.Linq;

public class GrammarReader
{
    // Start is called before the first frame update
    public  GrammarReader()
    {
    }

    public static GrammarTree ReadGrammar(string path)
    {
        try
        {
            SrgsDocument Grammar = new SrgsDocument(path);
            return ParseGrammar(Grammar);
        }
        catch
        {
            throw new Exception("Path does not lead to a SRGS Grammar: " + path);
        }
    }

    public static void WriteGrammar(string path, SrgsDocument document)
    {
        string srgsDocumentFile = Path.Combine(path, "srgsDocumentFile.xml");
        Debug.Log(srgsDocumentFile);
        // Create an XmlWriter object and pass the file path.
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(srgsDocumentFile, settings);

        // Write the contents of the XmlWriter object to an SRGS-compatible XML file.  
        document.WriteSrgs(writer);
        writer.Close();
    }

    public static GrammarTree ParseGrammar(SrgsDocument grammarDoc)
    {
        var gt = new GrammarTree();
        ProcessSrgsRule(grammarDoc.Root, gt, grammarDoc);
        gt.Root = gt.Productions.Find(g => g.ID == grammarDoc.Root.Id);
        return gt;
    }


    private static GrammarElement ProcessSrgsElement(SrgsElement element, GrammarTree grammar, SrgsDocument doc, string id)
    {
        if (element is SrgsText)
        {
            return ProcessSrgsText(element as SrgsText, grammar);
        }
        if (element is SrgsOneOf)
        {
            return ProcessSrgsOneOf(element as SrgsOneOf, grammar, doc, id);
        }
        if (element is SrgsItem)
        {
            return ProcessSrgsItem(element as SrgsItem, grammar, doc, id);
        }
        if (element is SrgsRuleRef)
        {
            return ProcessSrgsRuleRef(element as SrgsRuleRef, grammar, doc);
        }
        return new GrammarTerminal("");
    }

    private static GrammarTerminal ProcessSrgsText(SrgsText text, GrammarTree grammar)
    {

        if (!grammar.Terminals.Any( g => g.ID == text.Text))
        {
            grammar.Terminals.Add(new GrammarTerminal(text.Text));
        }
        return grammar.Terminals.Find(g => g.ID == text.Text);
    }

    private static GrammarNonTerminal ProcessSrgsOneOf(SrgsOneOf oneOf, GrammarTree grammar, SrgsDocument doc, string id)
    {
        if (!grammar.NonTerminals.Any(g => g.ID == id))
        {
            var node = new GrammarNonTerminal(id);
            grammar.NonTerminals.Add(node);
            for (int i = 0; i < oneOf.Items.Count; i++)
            {
                node.AppendNode(ProcessSrgsItem(oneOf.Items[i], grammar, doc, id + "." + i));
            }
        }
        
        return grammar.NonTerminals.Find(g => g.ID == id);
    }

    private static GrammarElement ProcessSrgsItem(SrgsItem item, GrammarTree grammar, SrgsDocument doc, string id)
    {
        bool b = id.StartsWith(doc.Root.Id.Trim('#')); // don not erase this line

        if (item.Elements.Count == 1 && b)
        {
            return ProcessSrgsElement(item.Elements[0], grammar, doc, id);
        }


        if (!grammar.Productions.Any(g => g.ID == id))
        {
            var pn = new GrammarProduction(id);
            grammar.Productions.Add(pn);
            for(int i = 0; i < item.Elements.Count; i++)
            {
                pn.AppendNode(ProcessSrgsElement(item.Elements[i], grammar, doc, id + "." + i));
            }
        }

        return grammar.Productions.Find(g => g.ID == id);
    }

    private static GrammarProduction ProcessSrgsRuleRef(SrgsRuleRef ruleRef, GrammarTree grammar, SrgsDocument doc)
    {
        doc.Rules.TryGetValue(ruleRef.Uri.ToString().Trim('#'), out SrgsRule r);

        return ProcessSrgsRule(r, grammar, doc);
    }

    private static GrammarProduction ProcessSrgsRule(SrgsRule rule, GrammarTree grammar, SrgsDocument doc)
    {
        if (!grammar.Productions.Any(g => g.ID == rule.Id))
        {
            var r = new GrammarProduction(rule.Id);
            grammar.Productions.Add(r);
            for(int i = 0; i < rule.Elements.Count; i++)
            {
                r.AppendNode(ProcessSrgsElement(rule.Elements[i], grammar, doc, rule.Id + "." + i));
            }
        }

        return grammar.Productions.Find(g => g.ID == rule.Id);
    }

}

