# RE4-ITM-TOOL
Extract and repack RE4 ITM files (RE4 ubisoft/2007/steam/uhd/Ps2)

**Translate from Portuguese Brazil**

Programa destinado a extrair e reempacotar arquivos .ITM
<br> Ao extrair será gerado um arquivo de extenção .idxitm, ele será usado para o repack.

**Update V.1.0.2**
<br>Nessa nova versão, para arquivos "0000.ITM", irá gerar uma pasta de nome "0000_ITM", mudança feita para evitar sobreposição de arquivos.

## Extract
Exemplo:
<br>RE4_ITM_TOOL.exe "r10a_009.ITM"

! Ira gerar um arquivo de nome "r10a_009.idxitm";
<br>! Irá criar uma pasta de nome "r10a_009";
<br>! Na pasta vão conter os arquivos que estavam dentro do ITM;

## Arquivo .idxitm

No arquivo idxitm, os itens inseridos são definidos da forma abaixo:
<br>**ID_0x1A:itm01a**
<br> Onde:
<br>* **ID_0x1A** -> esse é o ID do item no qual vai ser atribuído o modelo;
<br>* **itm01a** -> esse é o nome do arquivo que está na pasta, mas sem ser informado o formato .bin e .tpl, mas na pasta os arquivos devem estar com o formato do arquivo identificado;

## Repack
Exemplo:
<br>RE4_ITM_TOOL.exe "r10a_009.idxitm"

! No arquivo .idxitm vai conter os nomes dos arquivos que vão ser colocados no ITM;
<br>! Os arquivos têm que estar em uma pasta do mesmo nome do idxitm. Ex: "r10a_009";
<br>! No arquivo .idxitm as linhas iniciadas com um dos caracteres **# / \\ :** são consideradas comentários;
<br>! O nome do arquivo gerado é o mesmo nome do idxitm, mas com a extenção .ITM;

**At.te: JADERLINK**
<br>2024-08-17