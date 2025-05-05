.PHONY: synth

synth: 
	cdktf synth

diff:
	TF_LOG=debug cdktf plan

apply:
	TF_LOG=debug cdktf apply