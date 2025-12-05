-> inicio

=== inicio ===
#speaker:Beija-Flor
#portrait:BirdHappy
#audio:BirdVoice
EI! EI! VOCÊ! Felino listrado! Você tá vivo? Tá bem? Tá perdido? Eu posso ajudar! Aliás, eu VOU ajudar! Vamos? Vamos?

* [Você me conhece?] -> bom
* [Me deixa em paz] -> ruim

=== bom ===
#speaker:Maracajá
#portrait:MaracajaAngry
#audio:MaracajaVoice
Você me conhece por acaso?

#speaker:Beija-Flor
#portrait:BirdNeutral
#audio:BirdVoice
Conhecer? Não! Mas eu vi você ANDANDO igual alguém que precisa de ajuda! Então eu vou ajudar! #outcome:bom_bird

-> END

=== ruim ===
#speaker:Maracajá
#portrait:MaracajaAngry
#audio:MaracajaVoice
Me deixe em paz, passarinho barulhento.

#speaker:Beija-Flor
#portrait:BirdScared
#audio:BirdVoice
UOOOU! Gato bravo detectado! Mas tá tudo bem! Eu ajudo mesmo assim! Eu não deixo ninguém pra trás!

#speaker:Beija-Flor
#portrait:BirdUpset
#audio:BirdVoice
Esse seu jeito aí... um dia vai te cansar. #outcome:ruim_bird

-> END
