-> inicio
#MARACAJÁ ESTÁ SOZINHO
=== inicio ===
#speaker:Jabuti
#portrait:JabutiNeutral
#audio:JabutiVoice
Olá... maracujá.

#speaker:Maracajá
#portrait:MaracajaAngry
#audio:MaracajaVoice
É MARACAJÁ!

* [Desculpe... pode me orientar?] -> desculpar
* [Fale direito comigo.] -> corrigir

=== desculpar ===
#DeadWoods:2
#speaker:Maracajá
#portrait:MaracajaNeutral
#audio:MaracajaVoice
Desculpe... eu me exaltei um pouco. Poderia me orientar?

#speaker:Jabuti
#portrait:JabutiNeutral
#audio:JabutiVoice
Posso sim... caminhe comigo, maracujá. #outcome:jabuti_segue

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
Então siga sozinho... maracujá. #outcome:jabuti_nao_segue

-> END
