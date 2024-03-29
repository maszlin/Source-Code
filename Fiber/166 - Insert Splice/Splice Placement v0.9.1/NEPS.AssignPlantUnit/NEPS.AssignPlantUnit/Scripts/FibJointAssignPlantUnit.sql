SPOOL "C:\Temp\FibJointAssignPlantUnit.LOG";

SET DEFINE OFF;

SET SERVEROUTPUT ON;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * PROJECT NAME 	: NEPS.FibJointAssignPlantUnit
 * FUNCTIONALITY 	: Fiber Joint Assign Plant Unit
 * REFF DOCUMENT	: 
 * PROPOSED SQL		: TO REGISTER DLL TO GTECH SYSTEM
 * ASSEMBY NAME		: NEPS.FibJointAssignPlantUnit
 * NAMESPACE		: NEPS.FibJointAssignPlantUnit
 * CREATED BY		: M.ZAM
 * CREATED DATE		: 05 SEP 2012
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

	select G3E_FUNCTIONALINTERFACE_SEQ.nextval INTO v_G3E_FINO from dual ;

	DBMS_OUTPUT.PUT_LINE('G3E_FINO = '||v_G3E_FINO);
	
	-- REGISTER DLL INTO G3E_FUNCTIONALINTERFACE
	INSERT INTO G3E_FUNCTIONALINTERFACE (G3E_FINO, G3E_USERNAME, G3E_INTERFACE, G3E_ARGUMENTPROMPT, G3E_ARGUMENTTYPE, G3E_EDITDATE, G3E_PRJNO, G3E_DESCRIPTION)
	VALUES (v_G3E_FINO, 'Fiber Joint Assign Plant Unit', 'NEPS.FibJointAssignPlantUnit:NEPS.AssignPlantUnit.GTAssignPlantUnit', NULL, NULL, sysdate, NULL, 'Fiber Joint Assign Plant Unit');


	-- REGISTER G3E_FINO INTO G3E_ATTRIBUTE
	UPDATE G3E_ATTRIBUTE SET G3E_FINO = v_G3E_FINO, G3E_FUNCTIONALORDINAL = 1, G3E_FUNCTIONALTYPE = 'AddNew' WHERE G3E_ANO = 
		(SELECT G3E_ANO FROM G3E_ATTRIBUTE WHERE G3E_CNO = 11820 AND G3E_FIELD = 'G3E_GEOMETRY');
		
	--COMMIT;
	
	
END;
/

SET SERVEROUTPUT OFF;

SELECT G3E_FINO, G3E_USERNAME, G3E_INTERFACE FROM G3E_FUNCTIONALINTERFACE ;

SELECT G3E_CNO,G3E_table FROM G3E_COMPONENT WHERE G3E_CNO IN 
  (SELECT G3E_CNO FROM G3E_FEATURECOMPONENT WHERE G3E_FNO=11800);
--11820 GC_FSPLICE_S

SELECT G3E_CNO,G3E_ANO,G3E_FIELD FROM G3E_ATTRIBUTE WHERE G3E_CNO=11820;
--11820    1182006 G3E_GEOMETRY


SPOOL OFF;

