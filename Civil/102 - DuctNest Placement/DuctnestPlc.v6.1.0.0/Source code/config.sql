grant select on G3E_DUCTIFACE to designer;
grant select on G3E_DUCTFORMATION to designer;

CREATE PUBLIC SYNONYM G3E_DUCTFORMATION FOR NEPS.G3E_DUCTFORMATION;
CREATE PUBLIC SYNONYM G3E_DUCTIFACE FOR NEPS.G3E_DUCTIFACE;

update G3E_DUCTIFACE set G3E_PARAMETER1=0.235 where G3E_TYPE in ('G3E_DHSPACING','G3E_DVSPACING');
update G3E_DUCTIFACE set G3E_PARAMETER1=0.1 where G3E_TYPE in ('G3E_DHOFFSET','G3E_DHOFFSET');
commit;

delete from G3E_DUCTFORMATION where G3E_DFNO=2300;
delete from G3E_DUCTFORMATION where G3E_DFNO=2100;

DECLARE
   i NUMBER;
BEGIN
	i := 0;
	FOR rowCnt IN 1..48 LOOP
		FOR colCnt IN 1..48 LOOP
		   if (rowCnt * colCnt) <= 48 then
		   		Insert into G3E_DUCTFORMATION
					   (G3E_DFROWNO, 
					    G3E_DFNO, G3E_USERNAME, G3E_NUMROWS, G3E_NUMCOLS, G3E_EDITDATE, 
					    G3E_LOCALECOMMENT)
					 Values
					   ((select nvl(max(G3E_DFROWNO),0)+1 from G3E_DUCTFORMATION), 2300, rowCnt||' X '||colCnt, rowCnt, colCnt, 
					    SYSDATE, NULL);
					COMMIT;
					i := i + 1;
		   end if;
		END LOOP;
	END LOOP;
END;
/
