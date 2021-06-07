/*
silverPDF is sponsored by Aleyant Systems (http://www.aleyant.com)

silverPDF is based on PdfSharp (http://www.pdfsharp.net) and iTextSharp (http://itextsharp.sourceforge.net)

Developers: Ai_boy (aka Oleksii Okhrymenko)

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above information and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR SPONSORS
BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

*/
using System;
using System.IO;
using System.Collections;
using System.util;

namespace iTextSharp.text.pdf {

/**
 * <CODE>PdfArray</CODE> is the PDF Array object.
 * <P>
 * An array is a sequence of PDF objects. An array may contain a mixture of object types.
 * An array is written as a left square bracket ([), followed by a sequence of objects,
 * followed by a right square bracket (]).<BR>
 * This object is described in the 'Portable Document Format Reference Manual version 1.3'
 * section 4.6 (page 40).
 *
 * @see        PdfObject
 */

public class PdfArray : PdfObject {
    
    // membervariables
    
/** this is the actual array of PdfObjects */
    protected ArrayList arrayList;
    
    // constructors
    
/**
 * Constructs an empty <CODE>PdfArray</CODE>-object.
 */
    
    public PdfArray() : base(ARRAY) {
        arrayList = new ArrayList();
    }
    
/**
 * Constructs an <CODE>PdfArray</CODE>-object, containing 1 <CODE>PdfObject</CODE>.
 *
 * @param    object        a <CODE>PdfObject</CODE> that has to be added to the array
 */
    
    public PdfArray(PdfObject obj) : base(ARRAY) {
        arrayList = new ArrayList();
        arrayList.Add(obj);
    }
    
    public PdfArray(float[] values) : base(ARRAY) {
        arrayList = new ArrayList();
        Add(values);
    }
    
    public PdfArray(int[] values) : base(ARRAY) {
        arrayList = new ArrayList();
        Add(values);
    }
    
    /**
     * Constructs a PdfArray with the elements of an ArrayList.
     * Throws a ClassCastException if the ArrayList contains something
     * that isn't a PdfObject.
     * @param   l   an ArrayList with PdfObjects
     * @since 2.1.3
     */
    public PdfArray(ArrayList l) : this() {
        foreach (PdfObject o in l)
            Add(o);
    }

/**
 * Constructs an <CODE>PdfArray</CODE>-object, containing all the <CODE>PdfObject</CODE>s in a given <CODE>PdfArray</CODE>.
 *
 * @param    array        a <CODE>PdfArray</CODE> that has to be added to the array
 */
    
    public PdfArray(PdfArray array) : base(ARRAY) {
        arrayList = new ArrayList(array.arrayList);
    }
    
    // methods overriding some methods in PdfObject
    
/**
 * Returns the PDF representation of this <CODE>PdfArray</CODE>.
 *
 * @return        an array of <CODE>byte</CODE>s
 */
    
    //public override void ToPdf(PdfWriter writer, Stream os) {
    //    os.WriteByte((byte)'[');
    //    bool first = true;
    //    PdfObject obj = null;
    //    foreach (PdfObject obja in arrayList) {
    //        obj = (obja == null) ? PdfNull.PDFNULL : obja;
    //        type = obj.Type;
    //        if (!first && type != PdfObject.ARRAY && type != PdfObject.DICTIONARY && type != PdfObject.NAME && type != PdfObject.STRING)
    //            os.WriteByte((byte)' ');
    //        first = false;
    //        obj.ToPdf(writer, os);
    //    }
    //    os.WriteByte((byte)']');
    //}
    
    // methods concerning the ArrayList-membervalue
    
    // ARRAY CONTENT METHODS
    
    /**
     * Overwrites a specified location of the array.
     * 
     * @param idx The index of the element to be overwritten
     * @param obj new value for the specified index
     * @throws IndexOutOfBoundsException if the specified position doesn't exist
     * @return the previous value
     * @since 2.1.5
     */
    public PdfObject this[int idx] {
        get {
            return (PdfObject)arrayList[idx];
        }
        set {
            arrayList[idx] = value;
        }
    }

    /**
     * Remove the element at the specified position from the array.
     * 
     * Shifts any subsequent elements to the left (subtracts one from their
     * indices).
     * 
     * @param idx The index of the element to be removed.
     * @throws IndexOutOfBoundsException the specified position doesn't exist
     * @since 2.1.5
     */
    public PdfObject Remove(int idx) {
        PdfObject tmp = (PdfObject)arrayList[idx];
        arrayList.RemoveAt(idx);
        return tmp;
    }

/**
 * Returns an ArrayList containing <CODE>PdfObject</CODE>s.
 *
 * @return        an ArrayList
 */
    
    public ArrayList ArrayList {
        get {
            return arrayList;
        }
    }
    
/**
 * Returns the number of entries in the array.
 *
 * @return        the size of the ArrayList
 */
    
    public int Size {
        get {
            return arrayList.Count;
        }
    }
    
    /**
     * Returns <CODE>true</CODE> if the array is empty.
     * 
     * @return <CODE>true</CODE> if the array is empty
     * @since 2.1.5
     */
    public bool IsEmpty() {
        return arrayList.Count == 0;
    }

/**
 * Adds a <CODE>PdfObject</CODE> to the <CODE>PdfArray</CODE>.
 *
 * @param        object            <CODE>PdfObject</CODE> to add
 * @return        <CODE>true</CODE>
 */
    
    public virtual bool Add(PdfObject obj) {
        arrayList.Add(obj);
        return true;
    }
    
    public virtual bool Add(float[] values) {
        for (int k = 0; k < values.Length; ++k)
            arrayList.Add(new PdfNumber(values[k]));
        return true;
    }
    
    public virtual bool Add(int[] values) {
        for (int k = 0; k < values.Length; ++k)
            arrayList.Add(new PdfNumber(values[k]));
        return true;
    }
    
    /**
     * Inserts the specified element at the specified position.
     * 
     * Shifts the element currently at that position (if any) and
     * any subsequent elements to the right (adds one to their indices).
     *
     * @param index The index at which the specified element is to be inserted
     * @param element The element to be inserted
     * @throws IndexOutOfBoundsException if the specified index is larger than the
     *   last position currently set, plus 1. 
     * @since 2.1.5
     */
    public virtual void Add(int index, PdfObject element) {
        arrayList.Insert(index, element);
    }

    /**
     * Inserts a <CODE>PdfObject</CODE> at the beginning of the
     * <CODE>PdfArray</CODE>.
     * 
     * The <CODE>PdfObject</CODE> will be the first element, any other elements
     * will be shifted to the right (adds one to their indices).
     *
     * @param object The <CODE>PdfObject</CODE> to add
     */
    public virtual void AddFirst(PdfObject obj) {
        arrayList.Insert(0, obj);
    }
    
/**
 * Checks if the <CODE>PdfArray</CODE> allready contains a certain <CODE>PdfObject</CODE>.
 *
 * @param        object            <CODE>PdfObject</CODE> to check
 * @return        <CODE>true</CODE>
 */
    
    public bool Contains(PdfObject obj) {
        return arrayList.Contains(obj);
    }

    //public ListIterator GetListIterator() {
    //    return new ListIterator(arrayList);
    //}

    public override string ToString() {
        return arrayList.ToString();
    }

//    public PdfObject GetPdfObject( int idx ) {
//        return (PdfObject)arrayList[idx];
//    }
    
    public PdfObject GetDirectObject( int idx ) {
        throw new Exception();
        //return PdfReader.GetPdfObject(this[idx]);
    }
    
    // more of the same like PdfDictionary. (MAS 2/17/06)
    public PdfDictionary GetAsDict(int idx) {
        PdfDictionary dict = null;
        PdfObject orig = GetDirectObject(idx);
        if (orig != null && orig.IsDictionary())
            dict = (PdfDictionary) orig;
        return dict;
    }
    
    public PdfArray GetAsArray(int idx) {
        PdfArray array = null;
        PdfObject orig = GetDirectObject(idx);
        if (orig != null && orig.IsArray())
            array = (PdfArray) orig;
        return array;
    }
    
    //public PdfStream GetAsStream(int idx) {
    //    PdfStream stream = null;
    //    PdfObject orig = GetDirectObject(idx);
    //    if (orig != null && orig.IsStream())
    //        stream = (PdfStream) orig;
    //    return stream;
    //}
    
    public PdfString GetAsString(int idx) {
        PdfString str = null;
        PdfObject orig = GetDirectObject(idx);
        if (orig != null && orig.IsString())
            str = (PdfString) orig;
        return str;
    }
    
    public PdfNumber GetAsNumber(int idx) {
        PdfNumber number = null;
        PdfObject orig = GetDirectObject(idx);
        if (orig != null && orig.IsNumber())
            number = (PdfNumber) orig;
        return number;
    }
    
    public PdfName GetAsName(int idx) {
        PdfName name = null;
        PdfObject orig = GetDirectObject(idx);
        if (orig != null && orig.IsName())
            name = (PdfName) orig;
        return name;
    }
    
    public PdfBoolean GetAsBoolean(int idx) {
        PdfBoolean b = null;
        PdfObject orig = GetDirectObject(idx);
        if (orig != null && orig.IsBoolean())
            b = (PdfBoolean) orig;
        return b;
    }
    
    //public PdfIndirectReference GetAsIndirectObject(int idx) {
    //    PdfIndirectReference refi = null;
    //    PdfObject orig = this[idx]; // not getDirect this time.
    //    if (orig != null && orig.IsIndirect())
    //        refi = (PdfIndirectReference) orig;
    //    return refi;
    //}
}
}