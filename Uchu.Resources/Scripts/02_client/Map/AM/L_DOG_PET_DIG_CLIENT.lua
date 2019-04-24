require('02_client/Map/General/PET_DIG_CLIENT')

local specificPetLOTs = { 3254, 5635, 5637 }

function onStartup(self,msg)
	setPetVariables(specificPetLOTs)
end