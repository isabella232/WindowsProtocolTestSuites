// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Protocols.TestTools.StackSdk.Asn1;

namespace Microsoft.Protocols.TestTools.StackSdk.ActiveDirectory.Adts.Asn1CodecV3
{
    /*
    AttributeList_element ::= SEQUENCE {
                type    AttributeDescription,
                vals    SET OF AttributeValue }
    */
    public class AttributeList_element : Asn1Sequence
    {
        [Asn1Field(0)]
        public AttributeDescription type { get; set; }
        
        [Asn1Field(1)]
        public Asn1SetOf<AttributeValue> vals { get; set; }
        
        public AttributeList_element()
        {
            this.type = null;
            this.vals = null;
        }
        
        public AttributeList_element(
         AttributeDescription type,
         Asn1SetOf<AttributeValue> vals)
        {
            this.type = type;
            this.vals = vals;
        }
    }
}

