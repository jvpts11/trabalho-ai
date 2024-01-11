Instruções:

Não há botões, esse é apenas um programa que gera um grid e utiliza um algoritmo de 
random walker generator para gerar um mapa e usa um algoritmo de A* para
encontrar o caminho mais próximo de um quadrado verde até um quadrado amarelo

Inicialmente a ideia era:

ter um mapa gerado de forma procedural em que haveria um quadrado verde e outro amarelo gerado
em um dos quadrados de caminho, o verde seria o inicio e o amarelo o fim

após isso, um círculo azul andaria do quadrado verde até o quadrado amarelo pelo caminho criado.

E haveria um quadrado vermelho usando uma máquina de estados para caso o quadrado azul estivesse próximo do vermelho
o vermelho iria até o quadrado azul e o removeria do mapa, caso não, ele iria ficar a andar pelo mapa até estar proximo do quadrado azul.

O que foi implementado no final:

Algoritmo de random walker generator

A*

O que não foi implementado:

o objeto funcional do quadrado vermelho no unity
o andar do circulo azuç
