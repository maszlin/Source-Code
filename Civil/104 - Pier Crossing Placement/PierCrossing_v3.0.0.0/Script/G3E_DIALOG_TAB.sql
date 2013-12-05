SELECT * FROM G3E_DIALOG WHERE G3E_FNO=3600

select * from g3e_dialogtab where g3e_dtno in ( 360009,5101,360199)

update G3E_DIALOG set g3e_type = 'Pier Cross' where g3e_fno = 3600 and g3e_type = 'Placement';
commit; 