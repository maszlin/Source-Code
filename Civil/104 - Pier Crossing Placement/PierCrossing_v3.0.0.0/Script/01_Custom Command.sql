Insert into G3E_CUSTOMCOMMAND
   (G3E_CCNO, 
    G3E_PRJNO, G3E_USERNAME, G3E_DESCRIPTION, G3E_AUTHOR, G3E_COMMENTS, 
    G3E_LARGEBITMAP, G3E_SMALLBITMAP, G3E_TOOLTIP, G3E_STATUSBARTEXT, G3E_COMMANDCLASS, 
    G3E_ENABLINGMASK, G3E_MODALITY, G3E_SELECTSETENABLINGMASK, G3E_MENUORDINAL, G3E_LOCALECOMMENT, 
    G3E_EDITDATE, G3E_INTERFACE)
 Values
   ((SELECT max (G3E_CCNO) + 1 from G3E_CUSTOMCOMMAND), 
    NULL, 'Pier Crossing', 'Pier Crossing', 'Antaragrafik Systems Sdn Bhd', NULL,
    0, 0, 'Pier Crossing', NULL, 1, 
    8388624, 0, 0, 1, 
    NULL, sysdate, 'NEPS.PierCrossing:NEPS.GTechnology.PierCrossing.GTPierCrossing');
    
COMMIT;


--SELECT * FROM G3E_CUSTOMCOMMAND WHERE G3E_USERNAME LIKE 'Pier Crossing';

--DESC G3E_METADATAVERSION;
--SELECT G3E_COMMENTS FROM G3E_METADATAVERSION WHERE g3e_comments IS NOT NULL;
--DELETE FROM G3E_METADATAVERSION WHERE g3e_comments IS NOT NULL;

--DESC G3E_CUSTOMCOMMAND;
--SELECT * FROM G3E_CUSTOMCOMMAND;

--desc g3e_dialog;

--SELECT * FROM G3E_DIALOG WHERE G3E_FNO=3600

--select * from g3e_dialogtab where g3e_dtno in ( 360009,5101,360199)

--update G3E_DIALOG set g3e_type = 'Pier Cross' where g3e_fno = 3600 and g3e_type = 'Placement'
--commit; 