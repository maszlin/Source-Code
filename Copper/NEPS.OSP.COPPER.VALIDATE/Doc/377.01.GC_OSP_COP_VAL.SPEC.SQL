CREATE OR REPLACE PACKAGE GC_OSP_COP_VAL
AS

/********************************************************************************
 ** PACKAGE GC_OSP_COP_VAL      									   	   	   **
 **                                                                            **
 ** Created by : Ageng Setiawan 											   **
 ** Created Date : 21 Oct 2011 									               **
 ********************************************************************************/

-- FOR CALLING FROM GC_NR_CONNECT PROPOSED
IS_G3E_FID 	GC_NR_CONNECT.G3E_FID%TYPE 	:= 0;
IS_G3E_FNO 	GC_NR_CONNECT.G3E_FNO%TYPE 	:= 0;
IS_IN_FID 	GC_NR_CONNECT.IN_FID%TYPE 	:= 0;
IS_IN_FNO 	GC_NR_CONNECT.IN_FNO%TYPE 	:= 0;
IS_OUT_FID 	GC_NR_CONNECT.OUT_FID%TYPE 	:= 0;
IS_OUT_FNO 	GC_NR_CONNECT.OUT_FNO%TYPE 	:= 0;

-- FOR AUTOMATIC COPPER DETAIL PROPOSED
IS_DET_G3E_FID			GC_DETAIL.G3E_FID%TYPE := 0;
IS_DET_G3E_FNO			GC_DETAIL.G3E_FNO%TYPE := 0;
IS_DET_DETAIL_USERNAME	GC_DETAIL.DETAIL_USERNAME%TYPE;
IS_DET_POINT_X			NUMBER;
IS_DET_POINT_Y			NUMBER;
IS_ITFACE_CLASS			GC_ITFACE.ITFACE_CLASS%TYPE;

FUNCTION GET_PROCESSID RETURN VARCHAR2;

FUNCTION IS_CODE_IN_NUMBER(p_CHAR	VARCHAR2) RETURN BOOLEAN;

FUNCTION GET_LAST_GEOM_POINT(p_GEOMETRY	MDSYS.SDO_GEOMETRY,
							 p_X_OR_Y	VARCHAR2) RETURN NUMBER;
							 
FUNCTION GET_LAST_GEOM_LINE( p_GEOMETRY	MDSYS.SDO_GEOMETRY,
							 p_X_OR_Y	VARCHAR2) RETURN NUMBER;

FUNCTION EXTRACT_LINE_TO_VECTOR(p_GEOMETRY MDSYS.SDO_GEOMETRY) RETURN VARCHAR2;

FUNCTION OFSET_GEOM_T ( p_GEOMETRY	MDSYS.SDO_GEOMETRY,
						p_TOT_OFSET	NUMBER) RETURN MDSYS.SDO_GEOMETRY;

PROCEDURE TRACE_DOWNSTREAM(v_TRACE_NAME IN VARCHAR2, v_TRACE_FID IN NUMBER, v_TRACE_CURSOR OUT G3E_DBTYPES.G3E_REF_CURSOR);

PROCEDURE TRACE_UPSTREAM(v_TRACE_NAME IN VARCHAR2, v_TRACE_FID IN NUMBER, v_TRACE_CURSOR OUT G3E_DBTYPES.G3E_REF_CURSOR);

FUNCTION NEXT_CHAR(p_INIT_CHAR IN VARCHAR2) RETURN VARCHAR2;

FUNCTION GET_TOTAL_LENGTH(p_ARR_VECTOR IN VARCHAR2) RETURN NUMBER;

FUNCTION SUPPORT_INSERT_JOINT(p_G3E_FID IN NUMBER) RETURN VARCHAR2;

FUNCTION CNO_13001_GET_DP_NUMBER(p_EXC_ABB		IN VARCHAR2,
								 p_ITFACE_CODE	IN VARCHAR2,
								 p_MAX_DP_NUM	IN VARCHAR2) RETURN VARCHAR2; -- p_MAX_DP_NUM IS TO HANDLE IF SYSTEM ALREADY KNOW MAX DP_NUM 
								 
FUNCTION CNO_10801_GET_CBLCODE RETURN VARCHAR2;

FUNCTION GET_CODE_CBL_T(p_CABLE_CLASS		VARCHAR2,
						p_CABLE_CODE		VARCHAR2,
						p_SOURCE_TYPE		VARCHAR2,
						p_EXC_ABB			VARCHAR2) RETURN VARCHAR2;

FUNCTION GET_VALUE_CBL_T (	p_CABLE_CLASS		VARCHAR2,
							p_TEXT_FORMAT		NUMBER,
							p_COPPER_SIZE		NUMBER,
							p_GAUGE				NUMBER,
							p_EFFECTIVE_PAIRS	NUMBER,
							p_CTYPE				VARCHAR2,
							p_PLACEMENT			VARCHAR2,
							p_TOTAL_LENGTH		NUMBER,
							p_RACK_NO			NUMBER,		-- NEED TO KNOW THE SOURCE VALUE? BASE ON RULEBASE DEFAULT = 1
							p_CABLE_CODE		VARCHAR2,
							p_RT_CODE			VARCHAR2,
							p_SUB_TERMCODE		VARCHAR2,	-- FOR TAIL/VERTICAL_CABLE
							p_NUMCABLES			NUMBER,		-- FOR TAIL/VERTICAL_CABLE
							p_MDF_NUM			VARCHAR2,	-- FOR TAIL/VERTICAL_CABLE
							p_VERT_NUM			VARCHAR2,	-- FOR TAIL/VERTICAL_CABLE
							p_LO_PR				NUMBER,		-- FOR TAIL/VERTICAL_CABLE
							p_HI_PR				NUMBER		-- FOR TAIL/VERTICAL_CABLE
						 ) RETURN VARCHAR2;

FUNCTION CNO_10301_CHK_FORMAT_CODE (p_ITFACE_CODE	IN VARCHAR2,
									p_ITFACE_CLASS	IN VARCHAR2) RETURN VARCHAR2;
						 
FUNCTION CNO_10301_GET_ITFACE_CODE (p_JOB_ID		IN VARCHAR2,
									p_ITFACE_CLASS	IN VARCHAR2) RETURN VARCHAR2;

FUNCTION CNO_10401_GET_LC_NUM(p_EXC_ABB		IN VARCHAR2,
							  p_CABLE_CODE	IN VARCHAR2) RETURN NUMBER;

FUNCTION CNO_7000_GET_DSIDE_CBLCODE(p_G3E_FID	IN NUMBER,
									p_IN_FID	IN NUMBER)RETURN VARCHAR2;

FUNCTION CNO_10800_GET_SPLICE_CLASS(p_CABLE_CLASS IN VARCHAR2) RETURN VARCHAR2;
									
PROCEDURE CNO_GET_INFO_FROM_NR (p_G3E_FID	IN NUMBER,
								p_G3E_FNO 	IN NUMBER,
								p_CBL_CODE	OUT VARCHAR2,
								p_CAB_CODE	OUT VARCHAR2);

PROCEDURE CNO_SET_INFO_FROM_NR (p_G3E_FID	IN NUMBER,
								p_G3E_FNO 	IN NUMBER,
								p_IN_FID	IN NUMBER,		-- IF HAVE VALUE
								p_IN_FNO	IN NUMBER,		-- IF HAVE VALUE
								p_OUT_FID	IN NUMBER,		-- IF HAVE VALUE
								p_OUT_FNO	IN NUMBER);		-- IF HAVE VALUE

PROCEDURE STORE_PAIR_COUNT (p_SIDE			IN VARCHAR2,	
							p_G3E_FID 		IN NUMBER,
							p_G3E_FNO		IN NUMBER,
							p_DESIGNATION	IN VARCHAR2,
							p_LO_PAIR		IN VARCHAR2,
							p_HI_PAIR		IN VARCHAR2,
							p_REFCUR 		OUT G3E_DBTYPES.G3E_REF_CURSOR);	
					
END GC_OSP_COP_VAL; 
/
