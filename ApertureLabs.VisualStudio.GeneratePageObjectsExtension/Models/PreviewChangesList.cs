using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Models
{
    public class PreviewChangesList : IVsSimplePreviewChangesList,
        IVsLiteTreeList
    {
        #region Fields

        private readonly List<object> changeList;

        #endregion

        #region Constructor

        public PreviewChangesList()
        {
            changeList = new List<object>();
        }

        #endregion

        #region Methods

        #region IVsLiteTreeList Implementation

        /// <summary>
        /// Returns the attributes of the current tree list.
        /// </summary>
        /// <param name="pFlags">[out] Pointer to a variable indicating attributes of the current tree list. Values are taken from the <see cref="T:Microsoft.VisualStudio.Shell.Interop._VSTREEFLAGS" /> enumeration.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        int IVsLiteTreeList.GetFlags(out uint pFlags)
        {
            pFlags = (uint)_VSTREEFLAGS.TF_NOEVERYTHING;

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Returns the number of items in the preview list.
        /// </summary>
        /// <param name="pCount">[out] Returns the number of items in the preview list.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        int IVsLiteTreeList.GetItemCount(out uint pCount)
        {
            pCount = (uint)changeList.Count;

            return VSConstants.S_OK;
        }

        int IVsLiteTreeList.GetExpandedList(uint index,
            out int pfCanRecurse,
            out IVsLiteTreeList pptlNode)
        {
            var node = changeList[(int)index];
            pfCanRecurse = 0;
            pptlNode = null;

            return VSConstants.E_FAIL;
        }

        int IVsLiteTreeList.LocateExpandedList(IVsLiteTreeList ExpandedList, out uint iIndex)
        {
            throw new NotImplementedException();
        }

        int IVsLiteTreeList.OnClose(VSTREECLOSEACTIONS[] ptca)
        {
            throw new NotImplementedException();
        }

        int IVsLiteTreeList.GetText(uint index, VSTREETEXTOPTIONS tto, out string ppszText)
        {
            throw new NotImplementedException();
        }

        int IVsLiteTreeList.GetTipText(uint index, VSTREETOOLTIPTYPE eTipType, out string ppszText)
        {
            throw new NotImplementedException();
        }

        int IVsLiteTreeList.GetExpandable(uint index, out int pfExpandable)
        {
            throw new NotImplementedException();
        }

        int IVsLiteTreeList.GetDisplayData(uint index, VSTREEDISPLAYDATA[] pData)
        {
            throw new NotImplementedException();
        }

        int IVsLiteTreeList.UpdateCounter(out uint pCurUpdate, out uint pgrfChanges)
        {
            throw new NotImplementedException();
        }

        int IVsLiteTreeList.GetListChanges(ref uint pcChanges, VSTREELISTITEMCHANGE[] prgListChanges)
        {
            throw new NotImplementedException();
        }

        int IVsLiteTreeList.ToggleState(uint index, out uint ptscr)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVsSimplePreviewChangesList Implemenation

        int IVsSimplePreviewChangesList.GetItemCount(out uint pCount)
        {
            throw new NotImplementedException();
        }

        int IVsSimplePreviewChangesList.GetDisplayData(uint index, VSTREEDISPLAYDATA[] pData)
        {
            throw new NotImplementedException();
        }

        int IVsSimplePreviewChangesList.GetTextWithOwnership(uint index, VSTREETEXTOPTIONS tto, out string pbstrText)
        {
            throw new NotImplementedException();
        }

        int IVsSimplePreviewChangesList.GetTipTextWithOwnership(uint index, VSTREETOOLTIPTYPE eTipType, out string pbstrText)
        {
            throw new NotImplementedException();
        }

        int IVsSimplePreviewChangesList.GetExpandable(uint index, out int pfExpandable)
        {
            throw new NotImplementedException();
        }

        int IVsSimplePreviewChangesList.GetExpandedList(uint index, out int pfCanRecurse, out IVsSimplePreviewChangesList ppIVsSimplePreviewChangesList)
        {
            throw new NotImplementedException();
        }

        int IVsSimplePreviewChangesList.LocateExpandedList(IVsSimplePreviewChangesList pIVsSimplePreviewChangesListChild, out uint piIndex)
        {
            throw new NotImplementedException();
        }

        int IVsSimplePreviewChangesList.ToggleState(uint index, out uint ptscr)
        {
            throw new NotImplementedException();
        }

        int IVsSimplePreviewChangesList.OnRequestSource(uint index, object pIUnknownTextView)
        {
            throw new NotImplementedException();
        }

        int IVsSimplePreviewChangesList.OnClose(VSTREECLOSEACTIONS[] ptca)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
