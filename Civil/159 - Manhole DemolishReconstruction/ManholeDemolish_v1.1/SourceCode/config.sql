Insert into G3E_CUSTOMCOMMAND
   (G3E_CCNO, G3E_USERNAME, G3E_LARGEBITMAP, G3E_SMALLBITMAP, G3E_TOOLTIP, G3E_COMMANDCLASS, G3E_ENABLINGMASK, G3E_MODALITY, G3E_SELECTSETENABLINGMASK, G3E_MENUORDINAL, G3E_EDITDATE, G3E_INTERFACE)
 Values
   (G3E_CUSTOMCOMMAND_SEQ.nextval, 'Manhole Demolish/Reconstruction', 
    0, 0, 'Manhole Demolish/Reconstruction', 
    4, 8454160, 0, 0, 1, TO_DATE('03/14/2012 18:53:52', 'MM/DD/YYYY HH24:MI:SS'), 'NEPS.ManholeDemolish:AG.GTechnology.ManholeDemolish.GTManholeDemolish');
COMMIT;
