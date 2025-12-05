-> inicio
#MARACAJÁ ESTÁ COM O SARUÊ
=== inicio ===

#speaker:Saruê
#portrait:SarueNeutral
#audio:SarueVoice
M-maracajá... alguém está vindo... eu... eu não gosto disso.

#speaker:Jabuti
#portrait:JabutiNeutral
#audio:JabutiVoice
Olá... maracujá.

#speaker:Saruê
#portrait:SarueNeutral
#audio:SarueVoice
AHH! Quem é esse?

#speaker:Maracajá
#portrait:MaracajaAngry
#audio:MaracajaVoice
É MARACAJÁ!

* [Desculpe... pode me orientar?] -> desculpar
* [Fale direito comigo.] -> corrigir

=== desculpar ===
#DeadWoods:3
#speaker:Maracajá
#portrait:MaracajaNeutral
#audio:MaracajaVoice
Desculpe... eu me exaltei um pouco. Poderia me orientar?

#speaker:Jabuti
#portrait:JabutiNeutral
#audio:JabutiVoice
Posso sim... caminhe comigo, maracujá.

#speaker:Saruê
#portrait:SarueNeutral
#audio:SarueVoice
O-okay... c-caminhar... tudo bem... eu confio no velhinho... #outcome:jabuti_segue

-> END

=== corrigir ===
#DeadWoods:4
#speaker:Maracajá
#portrait:MaracajaAngry
#audio:MaracajaVoice
Meu nome não é assim. Fale direito comigo.

#speaker:Jabuti
#portrait:JabutiNeutral
#audio:JabutiVoice
Então siga sozinho… maracujá.

#speaker:Saruê
#portrait:SarueNeutral
#audio:SarueVoice
M-maracajá... você assustou ele! 

#speaker:Saruê
#portrait:SarueNeutral
#audio:SarueVoice
Eu... eu vou ficar aqui... é mais seguro... #outcome:jabuti_nao_segue

-> END
