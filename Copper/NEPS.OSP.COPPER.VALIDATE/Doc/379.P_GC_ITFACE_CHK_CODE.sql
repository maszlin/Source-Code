CREATE OR REPLACE TRIGGER P_GC_ITFACE_CHK_CODE_BIUR
BEFORE INSERT OR UPDATE
ON NEPS.B$GC_ITFACE
REFERENCING NEW AS NEW OLD AS OLD
FOR EACH ROW
WHEN (
NEW.G3E_ID != -1 OR NEW.G3E_ID IS NULL
      )
DECLARE
BEGIN
	IF (GC_COMMON.IsServicesEvent(LTT_ADMIN.GETMODE,:NEW.LTT_STATUS, INSERTING, B$GC_ITFACE_PKG.v_NumEntries) = FALSE) THEN
		RETURN;
	END IF;

	GC_COMMON.EnableLTTCleanup('B$GC_ITFACE');
	
	IF :NEW.ITFACE_CLASS = 'SDF' THEN
		:NEW.PCAB_CODE := NULL;
		:NEW.ITFACE_CODE := :NEW.SDF_CODE;
	ELSIF :NEW.ITFACE_CLASS = 'PHANTOM CABINET' THEN
		:NEW.SDF_CODE := NULL;
		:NEW.ITFACE_CODE := :NEW.PCAB_CODE;
	ELSE
		:NEW.PCAB_CODE := NULL;
		:NEW.SDF_CODE := NULL;
	END IF;
	
	GC_COMMON.DisableLTTCleanup;
EXCEPTION
	WHEN OTHERS THEN GC_COMMON.RAISEEXCEPTION('P_GC_ITFACE_CHK_CODE_BIUR');
END P_GC_ITFACE_CHK_CODE_BIUR;
/


