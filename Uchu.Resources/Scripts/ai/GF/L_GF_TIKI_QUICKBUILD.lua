-- copy of L_AG_CONCERT_PROP_QUICKBUILD.lua (ON THE OBJECT BUILT THOUGH THE CHOICEBUILD)

function onStartup(self)
	--self:SetVar("groupID","TikiHeads")
	self:AddObjectToGroup{ group = "TikiHeads" }
	print("*******************NEW TIKI CREATED ************************!!!!!!!!!!!!!")
end


function onRebuildNotifyState( self, msg)  
     --print (msg.iState)  
     if ( msg.iState == 4 ) then -- the choice build is smashed   
		print("******************* TIKI SMASHED ************************!!!!!!!!!!!!!")
	 
        --self:GetParentObj().objIDParent:SetVar('TikiSet', false)  
         self:SetVar('TikiSet', false)  
        print('destroy ramp') 
 		--LEVEL:DestroySpawnerObjects("Ramp")	
		--LEVEL:ResetSpawner("Ramp")		
		local rampSpawner = LEVEL:GetSpawnerByName("Ramp")
		if rampSpawner then
			rampSpawner:SpawnerDestroyObjects()
			rampSpawner:SpawnerReset()
		end
		 
     end  
  
end