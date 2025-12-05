-> inicio
#MARACAJÁ ESTÁ SEM O JABUTI
=== inicio ===
#speaker:Anta
#portrait:AntaNeutral
#audio:AntaVoice
Olá, maracajá. Sua energia está confusa.

#speaker:Beija-Flor
#portrait:BirdNeutral
#audio:BirdVoice
Confusa nada! A gente tá salvando a floresta, né Maracajá?? Hein? Hein???

#speaker:Beija-Flor
#portrait:BirdNeutral
#audio:BirdVoice
Ele tá confuso não! Ele só anda rápido demais!

#speaker:Anta
#portrait:AntaNeutral
#audio:AntaVoice
Sinto ventos acelerados… e um coração inquieto.

#speaker:Maracajá
#portrait:MaracajaAngry
#audio:MaracajaVoice
Eu tô *tentando* manter o foco…

* [Sim. Vamos consertar tudo] -> bom
* [Me deixa em paz] -> ruim

=== bom ===
#DriedRiver:2
#speaker:Maracajá
#portrait:MaracajaSmile
#audio:MaracajaVoice
Sim. Vamos consertar tudo.

#speaker:Anta
#portrait:AntaNeutral
#audio:AntaVoice
Vejo firmeza em suas palavras. Eu ajudarei.

#speaker:Beija-Flor
#portrait:BirdHappy
#audio:BirdVoice
Ela gostou de você! Eu sabia! Eu sabia!

#speaker:Anta
#portrait:AntaNeutral
#audio:AntaVoice
O pequeno carrega uma alegria… exagerada.

#speaker:Maracajá
#portrait:MaracajaNeutral
#audio:MaracajaVoice
Nem me fale... mas no fim ajuda. #outcome:bom_anta

-> END

=== ruim ===
#DriedRiver:4
#speaker:Maracajá
#portrait:MaracajaNeutral
#audio:MaracajaVoice
Fala menos, beija-flor!

#speaker:Beija-Flor
#portrait:BirdScared
#audio:BirdVoice
O QUÊ? Mas eu só tava tentando ajudar!

#speaker:Beija-Flor
#portrait:BirdSad
#audio:BirdVoice
Eu... só queria animar as coisas...

#speaker:Anta
#portrait:AntaNeutral
#audio:AntaVoice
A floresta sente quando há irritação. Ajudarei pouco.

#speaker:Maracajá
#portrait:MaracajaSad
#audio:MaracajaVoice
Tá... foi mal. Mas já era. #outcome:ruim_anta

-> END
