SPOOL "C:\Temp\NEPS.OSP.COPPER.VALIDATE.CopValidation.LOG";

SET DEFINE OFF;

SET SERVEROUTPUT ON;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * PROJECT NAME 	: COPPER FEATURE VALIDATION
 * FUNCTIONALITY 	: GET CHECK VALIDATION FOR ALL FEATURE
 * REFF DOCUMENT	: 
 * PROPOSED SQL		: TO REGISTER DLL TO GTECH SYSTEM
 * ASSEMBY NAME		: NEPS.OSP.COPPER.VALIDATE
 * NAMESPACE		: NEPS.OSP.COPPER.VALIDATE
 * CREATED BY		: AGENG SETIAWAN
 * CREATED DATE		: 16 JULY 2012 18:00 PM
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

DECLARE
	v_G3E_RINO 	G3E_RELATIONINTERFACE.G3E_RINO%TYPE := 7000;
BEGIN
	/*
	Name               	  	Null?    Type
	----------------------- -------- ------------------
	G3E_RINO           		NOT NULL NUMBER(9)
	G3E_USERNAME       		NOT NULL VARCHAR2(30)
	G3E_INTERFACE      		NOT NULL VARCHAR2(256)
	G3E_TYPE           		NOT NULL VARCHAR2(50)
	G3E_EDITDATE       		NOT NULL DATE
	G3E_PRJNO          		         NUMBER(9)
	G3E_DESCRIPTION    		         VARCHAR2(256)
	*/

	-- REGISTER DLL INTO G3E_FUNCTIONALINTERFACE
	INSERT INTO G3E_RELATIONINTERFACE (G3E_RINO, G3E_USERNAME, G3E_INTERFACE, G3E_TYPE, G3E_EDITDATE, G3E_PRJNO, G3E_DESCRIPTION)
	VALUES (v_G3E_RINO, 'Copper Feature Validation', 'NEPS.OSP.COPPER.VALIDATE:NEPS.OSP.COPPER.VALIDATE.CopValidation', 'Feature Validation', TO_DATE('07/16/2012 18:00:00', 'MM/DD/YYYY HH24:MI:SS'), NULL, 'Copper Feature Validation');
	COMMIT;
END;
/

SET SERVEROUTPUT OFF;

SPOOL OFF;
