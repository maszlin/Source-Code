CREATE OR REPLACE FUNCTION NEPS.AG_GET_CORECOUNT2(V_G3E_FNO NUMBER, G3E_FID NUMBER, V_FROM_FNO NUMBER, V_FROM_FID NUMBER , V_FROM_LOW NUMBER, V_FROM_HIGH NUMBER, V_TO_FNO number, V_TO_FID number,  V_TO_LOW NUMBER, V_TO_HIGH NUMBER, V_DEBUG integer default 0)
RETURN AG_CORECOUNT_TAB2 PIPELINED AS
    FNO1   NUMBER(5);
    FID1   NUMBER(10);
    LOW1   NUMBER(5);
    HIGH1  NUMBER(5);
    FNO2   NUMBER(5);
    FID2   NUMBER(10);
    LOW2   NUMBER(5);
    HIGH2  NUMBER(5);
	FNO_F_DCABLE NUMBER := 7400;
	FNO_F_SPLITTER NUMBER := 12300;
	item AG_CORECOUNT_ROW2;
	v_in_fno         NUMBER(5);
BEGIN
/*
	AUTHOR     : Catherine Lee
	Date       : Oct 2012
	Description: support core count module
	Version    : V3
 */
-- V_G3E_FNO > node, fplice : V_FROM_FNO > cable
IF ( FNO_F_DCABLE = V_FROM_FNO) or ( FNO_F_DCABLE = V_TO_FNO) or ((FNO_F_SPLITTER = V_FROM_FNO) AND (FNO_F_SPLITTER = V_TO_FNO)) THEN
	IF V_DEBUG>0 THEN
		DBMS_OUTPUT.PUT_LINE('Fiber Dside');
	END IF;
	--DBMS_OUTPUT.PUT_LINE('Fiber Dside');
    FOR item IN
	(SELECT * from TABLE (AG_GET_CORECOUNT_DSIDE2(V_FROM_FNO, V_FROM_FID, V_FROM_LOW, V_FROM_HIGH, V_TO_FNO, V_TO_FID,  V_TO_LOW, V_TO_HIGH, V_DEBUG)))
	LOOP
        --PIPE ROW( AG_CORECOUNT_ROW2( item.FNO1, item.FID1, item.LOW1, item.HIGH1, item.FNO2, item.FID2, item.LOW2, item.HIGH2,, item.TERM_FNO, item.TERM_FID, item.TERM_LOW, item.TERM_HIGH) );
		PIPE ROW(AG_CORECOUNT_ROW2( item.FNO1, item.FID1, item.LOW1, item.HIGH1, item.FNO2, item.FID2, item.LOW2, item.HIGH2,item.TERM_FNO, item.TERM_FID, item.TERM_LOW, item.TERM_HIGH) );

        --DBMS_OUTPUT.PUT_LINE( item.FNO1|| '-' ||item.FID1|| '-' ||lpad(item.LOW1,6)|| '-' ||lpad(item.HIGH1,6)|| '-' ||item.FNO2|| '-' ||item.FID2|| '-' ||lpad(item.LOW2,6)|| '-' ||lpad(item.HIGH2,6) || '-' ||item.TERM_FNO|| '-' ||item.TERM_FID|| '-' ||lpad(item.TERM_LOW,6)|| '-' ||lpad(item.TERM_HIGH,6) );
    END LOOP;

ELSE
	IF (V_DEBUG>0 )THEN
			DBMS_OUTPUT.PUT_LINE('Fiber Eside');
	END IF;
	--DBMS_OUTPUT.PUT_LINE('Fiber Eside');
	FOR item IN
	(SELECT * from TABLE (AG_GET_CORECOUNT_ESIDE2(V_FROM_FNO, V_FROM_FID, V_FROM_LOW, V_FROM_HIGH, V_TO_FNO, V_TO_FID,  V_TO_LOW, V_TO_HIGH, V_DEBUG)))
	LOOP
        --PIPE ROW( AG_CORECOUNT_ROW2( item.FNO1, item.FID1, item.LOW1, item.HIGH1, item.FNO2, item.FID2, item.LOW2, item.HIGH2,, item.TERM_FNO, item.TERM_FID, item.TERM_LOW, item.TERM_HIGH) );
		PIPE ROW(AG_CORECOUNT_ROW2( item.FNO1, item.FID1, item.LOW1, item.HIGH1, item.FNO2, item.FID2, item.LOW2, item.HIGH2,item.TERM_FNO, item.TERM_FID, item.TERM_LOW, item.TERM_HIGH) );

        --DBMS_OUTPUT.PUT_LINE( item.FNO1|| '-' ||item.FID1|| '-' ||lpad(item.LOW1,6)|| '-' ||lpad(item.HIGH1,6)|| '-' ||item.FNO2|| '-' ||item.FID2|| '-' ||lpad(item.LOW2,6)|| '-' ||lpad(item.HIGH2,6) || '-' ||item.TERM_FNO|| '-' ||item.TERM_FID|| '-' ||lpad(item.TERM_LOW,6)|| '-' ||lpad(item.TERM_HIGH,6) );
    END LOOP;
END IF;
--EXCEPTION WHEN OTHERS
--    THEN DBMS_OUTPUT.PUT_LINE(DBMS_UTILITY.FORMAT_STACK);
END;
/
