# RE4-ITM-TOOL
Extract and repack RE4 ITM files (RE4 2007/PS2/UHD/PS4/NS/GC/WII/XBOX360)

**Translate from Portuguese Brazil**

Programa destinado a extrair e reempacotar arquivos .ITM
<br> Ao extrair será gerado um arquivo de extenção .idxitm, ele será usado para o repack.

**Update V.1.1.0**
<br>Adicionado suporte para as versões GC/WII/XBOX360;

**Update V.1.0.2**
<br>Nessa nova versão, para arquivos "0000.ITM", irá gerar uma pasta de nome "0000_ITM", mudança feita para evitar sobreposição de arquivos.

## Extract
Exemplo:
<br>RE4_ITM_TOOL_*.exe "r10a_009.ITM"

! Vai gerar um arquivo de nome "r10a_009.idxitm";
<br>! Vai criar uma pasta de nome "r10a_009";
<br>! Na pasta vão conter os arquivos que estavam dentro do ITM;

## Arquivo .idxitm

No arquivo idxitm, os itens inseridos são definidos da forma abaixo:
<br>**ID_0x1A:itm01a**
<br> Onde:
<br>* **ID_0x1A** -> esse é o ID do item no qual vai ser atribuído o modelo;
<br>* **itm01a** -> esse é o nome do arquivo que está na pasta, mas sem ser informado o formato .bin e .tpl, mas na pasta os arquivos devem estar com o formato do arquivo identificado;

## Repack
Exemplo:
<br>RE4_ITM_TOOL_*.exe "r10a_009.idxitm"

! No arquivo .idxitm vai conter os nomes dos arquivos que vão ser colocados no ITM;
<br>! Os arquivos têm que estar em uma pasta do mesmo nome do idxitm. Ex: "r10a_009";
<br>! No arquivo .idxitm as linhas iniciadas com um dos caracteres **# / \\ :** são consideradas comentários;
<br>! O nome do arquivo gerado é o mesmo nome do idxitm, mas com a extenção .ITM;

## BIG_ENDIAN vs LITTLE_ENDIAN

! Para as versões "GC/WII/XBOX360" use a tool de nome BIG_ENDIAN;
<br>! Para as versões "2007/PS2/UHD/PS4/NS" use a tool de nome LITTLE_ENDIAN;

**At.te: JADERLINK**
<br>2024-12-27