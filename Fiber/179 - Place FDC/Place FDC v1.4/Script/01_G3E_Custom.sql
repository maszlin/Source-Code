SET DEFINE OFF;
Insert into NEPS.G3E_CUSTOMCOMMAND
   (G3E_CCNO, G3E_USERNAME, G3E_DESCRIPTION, G3E_AUTHOR, G3E_LARGEBITMAP, G3E_SMALLBITMAP, G3E_TOOLTIP, G3E_COMMANDCLASS, G3E_ENABLINGMASK, G3E_MODALITY, G3E_SELECTSETENABLINGMASK, G3E_MENUORDINAL, G3E_EDITDATE, G3E_INTERFACE)
 Values
   (G3E_CUSTOMCOMMAND_SEQ.nextval, 'Place FDC', 'Place FDC', 'Ghost',  0, 0, 'Place FDC', 
    1, 8388624, 0, 0, 1, sysdate, 'NEPS.PlaceFDC:NEPS.GTechnology.PlaceFDC.GTCustomCommandModeless');
	
COMMIT;





