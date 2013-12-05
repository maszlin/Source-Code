/*
*   CREATED : m.zam 23-09-2012
*   ISSUES  : connecting copper cable to fiber rt or msan
*   - add column RT_CODE to gc_dp, gc_pdp and ag_cable_joint
*	
*/


ALTER TABLE B$GC_DP ADD (RT_CODE VARCHAR2(9));
CREATE OR REPLACE VIEW GC_DP AS SELECT * FROM B$GC_DP;

EXEC CONSTRAINT_TRANSFER.GETPRIMARYCONSTRAINT ('B$GC_DP');
EXEC CONSTRAINT_TRANSFER.DROPUNIQUECONSTRAINT ('B$GC_DP');
EXEC CONSTRAINT_TRANSFER.DROPUNIQUECONSTRAINT ('B$GC_DP');
EXEC CONSTRAINT_TRANSFER.DROPUNIQUEINDEX ('B$GC_DP');
EXEC CREATE_VIEWS.CREATE_LTT_VIEW ('B$GC_DP');
EXEC CREATE_TRIGGERS.CREATE_LTT_TRIGGER ('B$GC_DP');
EXEC GDOTRIGGERS.CREATE_GDOTRIGGERS ('B$GC_DP');

INSERT INTO G3E_ATTRIBUTE (G3E_CNO, G3E_ANO, G3E_FIELD, G3E_USERNAME, G3E_REQUIRED)
VALUES (13001, 1300162, 'RT_CODE', 'RT CODE', 0);


ALTER TABLE B$GC_PDP ADD (RT_CODE VARCHAR2(9));
CREATE OR REPLACE VIEW GC_PDP AS SELECT * FROM B$GC_PDP;

EXEC CONSTRAINT_TRANSFER.GETPRIMARYCONSTRAINT ('B$GC_PDP');
EXEC CONSTRAINT_TRANSFER.DROPUNIQUECONSTRAINT ('B$GC_PDP');
EXEC CONSTRAINT_TRANSFER.DROPUNIQUECONSTRAINT ('B$GC_PDP');
EXEC CONSTRAINT_TRANSFER.DROPUNIQUEINDEX ('B$GC_PDP');
EXEC CREATE_VIEWS.CREATE_LTT_VIEW ('B$GC_PDP');
EXEC CREATE_TRIGGERS.CREATE_LTT_TRIGGER ('B$GC_PDP');
EXEC GDOTRIGGERS.CREATE_GDOTRIGGERS ('B$GC_PDP');

INSERT INTO G3E_ATTRIBUTE (G3E_CNO, G3E_ANO, G3E_FIELD, G3E_USERNAME, G3E_REQUIRED)
VALUES (13101, 1310127, 'RT_CODE', 'RT CODE', 0);


ALTER TABLE AG_CABLE_JOINT ADD RT_CODE VARCHAR(9);
CREATE PUBLIC SYNONYM AG_CABLE_JOINT FOR NEPS.AG_CABLE_JOINT;
GRANT SELECT ON AG_CABLE_JOINT TO DESIGNER;

COMMIT;

