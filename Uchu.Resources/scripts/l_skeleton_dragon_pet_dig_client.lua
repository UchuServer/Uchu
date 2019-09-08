require('PET_DIG_CLIENT')

local specificPetLOTs = { 13067 }
local noPetString = "SKELETON_ONLY_PET_DIG"
local noPetIcon   = 4105        

local missionRequirements = {
								{ 	
									ID	 	= 1299,
									state 	= 4,
									string	= "CP_DIG_COALESSA",
									icon	= 3794
								},
								{
									ID	 	= 1299,
									state 	= 1,
									string	= "CP_DIG_WU",
									icon	= 4002
								},
								{
									ID	 	= 1299,
									state 	= 0,
									string	= "CP_DIG_WU",
									icon	= 4002
								}
							}

function onRenderComponentReady(self,msg)
	setPetVariables(specificPetLOTs,noPetString,noPetIcon,missionRequirements)
end
