﻿/****************************************************************************
 * Copyright (c) 2017 maoling@putao.com
****************************************************************************/
using System.Xml;
using System.Collections.Generic;

namespace UBlockly
{
    [MutatorClass(MutatorId = "text_join_mutator;lists_create_with_item_mutator")]
    public class ItemListMutator : Mutator
    {
        private const string EMPTY_NAME = "EMPTY";
        private const string ADD_INPUT_PREFIX = "ADD";
        
        private int mItemCount = 2;
        public int ItemCount { get { return mItemCount; } }

        private string mLabelText;

        public override bool NeedEditor
        {
            get { return true; }
        }
        
        public void Mutate(int itemCount)
        {
            if (mItemCount == itemCount)
                return;
            
            mItemCount = itemCount;
            if (mBlock != null)
                UpdateInternal();
        }
        
        public override XmlElement ToXml()
        {
            XmlElement xmlElement = XmlUtil.CreateDom("mutation");
            xmlElement.SetAttribute("items", mItemCount.ToString());
            return xmlElement;
        }

        public override void FromXml(XmlElement xmlElement)
        {
            mItemCount = int.Parse(xmlElement.GetAttribute("items"));
            UpdateInternal();
        }

        protected override void OnAttached()
        {
            Input defaultInput = mBlock.InputList[0];
            defaultInput.SetName(EMPTY_NAME);
            FieldLabel field = defaultInput.FieldRow[0] as FieldLabel;
            mLabelText = field.GetText();
            UpdateInternal();
        }

        private void UpdateInternal()
        {
            // currently reserve the dummy input, it will only show the Label Field on UI
            Input emptyInput = mBlock.GetInput(EMPTY_NAME);
            if (mItemCount > 0 && emptyInput != null)
            {
                mBlock.RemoveInput(emptyInput);
            }
            else if (mItemCount == 0 && emptyInput == null)
            {
                emptyInput = InputFactory.Create(Define.EConnection.DummyInput, EMPTY_NAME, Define.EAlign.Right, null);
                emptyInput.AppendField(new FieldLabel(null, mLabelText));
                mBlock.AppendInput(emptyInput);
            }

            //add new inputs
            int i = 0;
            for (i = 0; i < mItemCount; i++)
            {
                Input addInput = mBlock.GetInput("ADD" + i);
                if (addInput == null)
                {
                    addInput = InputFactory.Create(Define.EConnection.InputValue, ADD_INPUT_PREFIX + i, Define.EAlign.Right, null);
                    mBlock.AppendInput(addInput);
                }
                if (i == 0)
                {
                    addInput.AppendField(new FieldLabel(null, mLabelText));
                }
            }

            // remove deleted inputs
            while (true)
            {
                Input addInput = mBlock.GetInput("ADD" + i);
                if (addInput == null)
                    break;

                mBlock.RemoveInput(addInput);
                i++;
            }
        }
    }
}
