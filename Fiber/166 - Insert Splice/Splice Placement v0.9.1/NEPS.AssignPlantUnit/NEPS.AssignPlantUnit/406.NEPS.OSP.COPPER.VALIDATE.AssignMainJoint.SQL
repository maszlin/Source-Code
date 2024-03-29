SPOOL "C:\Temp\NEPS.OSP.COPPER.VALIDATE.GetJointAttribute.LOG";

SET DEFINE OFF;

SET SERVEROUTPUT ON;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * PROJECT NAME 	: COPPER VALIDATE
 * FUNCTIONALITY 	: GET JOINT ATTRIBUTE
 * REFF DOCUMENT	: 
 * PROPOSED SQL		: TO REGISTER DLL TO GTECH SYSTEM
 * ASSEMBY NAME		: NEPS.OSP.COPPER.VALIDATE
 * NAMESPACE		: NEPS.OSP.COPPER.VALIDATE
 * CREATED BY		: M.ZAM
 * CREATED DATE		: 10 AUG 2012
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

DECLARE
	v_G3E_FINO 	G3E_FUNCTIONALINTERFACE.G3E_FINO%TYPE;
BEGIN
	/*
	Name                Null?    Type
	------------------- -------- -------------
	G3E_FINO            NOT NULL NUMBER(9)
	G3E_USERNAME        NOT NULL VARCHAR2(80)
	G3E_INTERFACE       NOT NULL VARCHAR2(256)
	G3E_ARGUMENTPROMPT           RAW(2000)
	G3E_ARGUMENTTYPE             RAW(2000)
	G3E_EDITDATE        NOT NULL DATE
	G3E_PRJNO                    NUMBER(9)
	G3E_DESCRIPTION              VARCHAR2(256)
	*/
	-- SELECT LATEST G3E_FINO
	SELECT MAX(G3E_FINO) + 1 INTO v_G3E_FINO FROM G3E_FUNCTIONALINTERFACE;
	DBMS_OUTPUT.PUT_LINE('G3E_FINO = '||v_G3E_FINO);
	
	-- REGISTER DLL INTO G3E_FUNCTIONALINTERFACE
	INSERT INTO G3E_FUNCTIONALINTERFACE (G3E_FINO, G3E_USERNAME, G3E_INTERFACE, G3E_ARGUMENTPROMPT, G3E_ARGUMENTTYPE, G3E_EDITDATE, G3E_PRJNO, G3E_DESCRIPTION)
	VALUES (v_G3E_FINO, 'Assign Main Joint', 'NEPS.AssignMainJoint:NEPS.AssignMainJoint.GTAssignMainJoint', NULL, NULL, sysdate, NULL, 'Assign Main Joint');
	COMMIT;
	
	-- REGISTER G3E_FINO INTO G3E_ATTRIBUTE
	UPDATE G3E_ATTRIBUTE SET G3E_FINO = v_G3E_FINO, G3E_FUNCTIONALORDINAL = 1, G3E_FUNCTIONALTYPE = 'Update' WHERE G3E_ANO = 
		(SELECT G3E_ANO FROM G3E_ATTRIBUTE WHERE G3E_CNO = 7001 AND G3E_FIELD = 'CABLE_CODE');
	COMMIT;
	
	
END;
/

SET SERVEROUTPUT OFF;

SPOOL OFF;
