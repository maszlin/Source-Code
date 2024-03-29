DROP TRIGGER P_GC_ITFACE_CHK_CODE;

CREATE OR REPLACE TRIGGER "NEPS"."P_GC_ITFACE_AUDIT_BIUR"
BEFORE INSERT OR UPDATE
ON NEPS.B$GC_ITFACE
REFERENCING NEW AS NEW OLD AS OLD
FOR EACH ROW
WHEN (
NEW.G3E_ID != -1 OR NEW.G3E_ID IS NULL
      )
DECLARE
	V_SESSION_USER VARCHAR2(30);
	V_IP_ADDRESS VARCHAR2(30);
	V_HOST VARCHAR2(50);
	V_OS_USER VARCHAR2(50);
BEGIN
	IF (GC_COMMON.IsServicesEvent(LTT_ADMIN.GETMODE,:NEW.LTT_STATUS, INSERTING, B$GC_ITFACE_PKG.v_NumEntries) = FALSE) THEN
		RETURN;
	END IF;

	GC_COMMON.EnableLTTCleanup('B$GC_ITFACE');

	SELECT SYS_CONTEXT ('USERENV', 'SESSION_USER') INTO V_SESSION_USER FROM dual;
	SELECT SYS_CONTEXT ('USERENV', 'IP_ADDRESS') INTO V_IP_ADDRESS FROM dual;
	SELECT SYS_CONTEXT ('USERENV', 'HOST') INTO V_HOST FROM dual;
	SELECT SYS_CONTEXT ('USERENV', 'OS_USER') INTO V_OS_USER FROM dual;

	IF INSERTING THEN
		:NEW.CREATED_BY := V_SESSION_USER;
		:NEW.CREATED_DATE := SYSDATE;
		:NEW.CREATED_IP_ADDRESS := V_IP_ADDRESS;
		:NEW.CREATED_HOST := V_HOST;
		:NEW.CREATED_OS_USER := V_OS_USER;
	END IF;
	IF UPDATING THEN
		:NEW.MODIFIED_BY := V_SESSION_USER;
		:NEW.MODIFIED_DATE := SYSDATE;
		:NEW.MODIFIED_IP_ADDRESS := V_IP_ADDRESS;
		:NEW.MODIFIED_HOST := V_HOST;
		:NEW.MODIFIED_OS_USER := V_OS_USER;
	END IF;

	GC_COMMON.DisableLTTCleanup;
EXCEPTION
	WHEN OTHERS THEN GC_COMMON.RAISEEXCEPTION('P_GC_ITFACE_AUDIT_BIUR');
END P_GC_ITFACE_AUDIT_BIUR;
/

