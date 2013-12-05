-- 12-09-2012 @ m.zam

-- add CAB_TYPE column into AG_CABLE_JOINT
/*
desc AG_CABLE_JOINT

 Name                                                                     Null?    Type--
 ------------------------------------------------------------------------ -------- --------------
 ID                                                                       NOT NULL NUMBER
 ACTIVE_JOB                                                                        VARCHAR2(30)
 CABLE_FID                                                                         NUMBER(10)
 JOINT_FID                                                                         NUMBER(10)
 ITFACE_CODE                                                                       VARCHAR2(30)
 CABLE_CODE                                                                        VARCHAR2(30)
 CTYPE                                                                             VARCHAR2(30)
 TOTAL_SIZE                                                                        NUMBER(38)
 GAUGE                                                                             NUMBER(4,2)
*/
 
ALTER TABLE AG_CABLE_JOINT ADD CAB_TYPE VARCHAR2(10);

