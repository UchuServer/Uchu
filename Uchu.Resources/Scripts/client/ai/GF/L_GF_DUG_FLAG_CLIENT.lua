function onRenderComponentReady(self, msg) 
	--print('onRenderComponentReady')
		--sets the flag invisible to start the map
	self:SetVisible{ visible = false }
end

function onPhysicsComponentReady(self, msg) 
	--sets the flag to not collidable with the player at the start of the map
	self:SetCollisionGroup{colGroup = 25}	
end


function onNotifyClientObject(self,msg)
	--print('%%%%%%%%%%%%%%%%%%%%%%% onNotifyClientObject @@@@@@@@@@@@@@@@@@@@@@@@@@')
	--notification from L_GF_PET_TREASURE_NODE 
	if msg.name == "changePhysics" then
	
		--set physics on
		--self:ActivatePhysics{bActive = true}
		self:SetCollisionGroup{ colGroup = 1 }
		--visibliy on
		--print('**************** the physics should change ****************************')
		self:SetVisible{visible = true }
	
	end


end