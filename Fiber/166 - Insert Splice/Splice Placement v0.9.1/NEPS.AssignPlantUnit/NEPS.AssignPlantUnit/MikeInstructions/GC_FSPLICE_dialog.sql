
-- 'GC_FSPLICE'

select * FROM g3e_dialog where g3e_fno=11800 ;


col G3E_TYPE form a30
col G3E_USERNAME form a30
select dt.G3E_DTNO, G3E_ORDINAL, dt.G3E_USERNAME, G3E_TYPE from G3E_DIALOGTAB dt, G3E_DIALOG d 
  where dt.g3e_dtno=d.g3e_dtno and g3e_fno=11800 order by g3e_dno, dt.G3E_DTNO, G3E_ORDINAL ;
/*
G3E_DTNO G3E_ORDINAL G3E_USERNAME                   G3E_TYPE
-------- ----------- ------------------------------ ----------
    5204          52 Address                        Placement
    5701          58 Core Count                     Placement
    6301          65 Hyperlink                      Placement
    8701          87 Splitter Core Count            Placement
 1180101           1 Fiber Splice Enclosure         Placement
 1180199         102 Audit                          Placement
*/


col G3E_FIELD form a15
col G3E_DEFAULT form a12
col G3E_TABLE form a18
select G3E_TANO, ta.G3E_PNO, G3E_TRIGGERPOPULATE, G3E_DEFAULT, G3E_CNO, a.G3E_ANO, G3E_FIELD 
  from G3E_TABATTRIBUTE ta, G3E_ATTRIBUTE a
  where ta.G3E_ANO=a.G3E_ANO and G3E_DTNO=1180101 ;
/*
 G3E_TANO    G3E_PNO G3E_TRIGGERPOPULATE G3E_DEFAULT     G3E_CNO    G3E_ANO G3E_FIELD
--------- ---------- ------------------- ------------ ---------- ---------- ---------------
118010104                              0                   11801    1180108 DESCRIPTION
118010105      11801                   1                      51       5114 MIN_MATERIAL
118010106                              0 TM                   51       5111 OWNERSHIP
118010109         31                   0                      51       5106 JOB_ID
118010110                              0 PROPOSED             51       5107 JOB_STATE
118010111                              0 PPF                  51       5108 FEATURE_STATE
118010113                              0 2012                 51       5112 YEAR_PLACED
118010101                              0                   11801    1180109 PHYSICAL_TYPE
118010102                              0 FUJIKURA          11801    1180106 MANUFACTURER
118010103                              0                   11801    1180107 MODEL
118010114      11802                   0                   11801    1180120 BRANCH_TYPE
118010115      11803                   0                   11801    1180121 CLOSURE_SIZE
118010117      50098                   0 APEX              11801    1180123 CONTRACTOR
118010108                              0                      51       5132 EXC_ABB
*/

col G3E_FIELD form a15
col G3E_DEFAULT form a12
col G3E_TABLE form a18
select G3E_TANO, ta.G3E_PNO, G3E_TABLE, G3E_TRIGGERPOPULATE, G3E_DEFAULT, G3E_CNO, a.G3E_ANO, G3E_FIELD 
  from G3E_TABATTRIBUTE ta, G3E_ATTRIBUTE a, G3E_PICKLIST pl
  where ta.G3E_ANO=a.G3E_ANO and ta.G3E_PNO=pl.G3E_PNO and G3E_DTNO=1180101 ;
/*
 G3E_TANO    G3E_PNO G3E_TABLE          G3E_TRIGGERPOPULATE G3E_DEFAULT     G3E_CNO    G3E_ANO G3E_FIELD
--------- ---------- ------------------ ------------------- ------------ ---------- ---------- ------------
118010105      11801 REF_FSPLICE                          1                      51       5114 MIN_MATERIAL
118010109         31 G3E_JOB                              0                      51       5106 JOB_ID
118010114      11802 REF_FSPLICE                          0                   11801    1180120 BRANCH_TYPE
118010115      11803 REF_FSPLICE                          0                   11801    1180121 CLOSURE_SIZE
118010117      50098 REF_FIB_CBLCONTR                     0 APEX              11801    1180123 CONTRACTOR
*/


select ta.G3E_TANO, ta.G3E_PNO, G3E_TRIGGERPOPULATE, G3E_DEFAULT, G3E_CNO, a.G3E_ANO, G3E_FIELD
  from G3E_AUTOPOPULATE ap, g3e_tabattribute ta, G3E_ATTRIBUTE a 
  where ta.G3E_ANO=a.G3E_ANO and ap.G3E_TANO=ta.G3E_TANO and G3E_DTNO =1180101 ; 


col G3E_TRIGGERFILTER form a40
select * from G3E_TRIGGERSET where G3E_SETNO in
 (select G3E_SETNO from G3E_AUTOPOPULATE where G3E_TANO in 
  (select G3E_TANO from g3e_tabattribute where G3E_DTNO =1180101 ) ) ; 

/*
  MIN_MATERIAL is used to auto populate BRANCH_TYPE n CLOSURE_SIZE
*/
