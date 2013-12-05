SELECT B.* FROM TRACEID A, TRACERESULT B WHERE A.G3E_ID = B.G3E_TNO AND A.G3E_NAME = 'ESIDE_DOWN_28JUNE2012_1114';
-- create GC_MSAN_TEMPLATE table
CREATE TABLE AG_MSAN_TEMPLATE
(
  ID			NUMBER PRIMARY KEY,
  MODEL_ID		NUMBER,
  RACK_NO		VARCHAR2(4),
  FRAME_NO      VARCHAR2(4),
  SLOT_NO		VARCHAR2(4),
  SLOT_NIS		VARCHAR2(12),
  CARD_TYPE		VARCHAR2(30),
  CARD_MODEL	VARCHAR2(30),
  PORT_LO		VARCHAR(4),
  PORT_HI		VARCHAR(4)
);

-- create GC_MSAN_XLTEMPLATE table
CREATE TABLE AG_MSAN_XLTEMPLATE
(
  MODEL_ID		NUMBER PRIMARY KEY,
  MANUFACTURER	VARCHAR2(30),			
  MODEL	  		VARCHAR2(30),			
  XL_FILE		VARCHAR2(100)  			
);


-- create auto increment sequence for the primary key
CREATE SEQUENCE MSAN_XL_SEQ
START WITH 1
INCREMENT BY 1;

-- create auto increment sequence for the primary key
CREATE SEQUENCE MSAN_TEMPLATE_SEQ
START WITH 1
INCREMENT BY 1;


CREATE OR REPLACE PUBLIC SYNONYM MSAN_XL_SEQ FOR NEPS.MSAN_XL_SEQ;
GRANT SELECT ON NEPS.MSAN_XL_SEQ TO DESIGNER;

CREATE OR REPLACE PUBLIC SYNONYM MSAN_TEMPLATE_SEQ FOR NEPS.MSAN_TEMPLATE_SEQ;
GRANT SELECT ON NEPS.MSAN_TEMPLATE_SEQ TO DESIGNER;




