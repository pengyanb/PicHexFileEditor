# PicHexFileEditor
Edit PIC microcontroller hex file directly. 
No need to rebuild the entire project. 

Typical scenario:
modify the hex file so that each microcontroller is programmed with unique information, for instance, username, password...

hex file Format for each line:

: CC AAAA TT D1 D2 D3 .... DN CS \r\n

CC    - Data Count
AAAA  - lower 16-bits of the address
TT    - Data type
D1~DN - Data bytes
CS    - Checksum = (~(CC+AA+AA+TT+D1+...+DN)) + 1
\r\n  - new line

refer to http://microsym.com/editor/assets/intelhex.pdf
