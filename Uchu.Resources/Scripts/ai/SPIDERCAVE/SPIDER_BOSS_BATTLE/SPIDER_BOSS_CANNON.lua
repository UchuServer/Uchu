--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

--------------------------------------------------
-- Spider Boss Start up --------------------------
--------------------------------------------------

function onStartup(self)
rebuilds = {}
static = {}

-- Event Delays 
static['Chance_ToHit_Player']  = 40
static['Chance_ToHit_ReBuild'] = 10

		self:SetVar("ON", false)
		--GAMEOBJ:GetTimer():AddTimerWithCancel( 20 , "fire", self )

	--GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "StoreActivity", self )
end
--------------------------------------------------------------
-- Called when the client wants to fire
--------------------------------------------------------------
function onShootingGalleryFire(self, msg)


	--------------------------------------
	--   Store Activity Object if nil   --
	--------------------------------------
	if not self:GetVar("ActivityObj") then

		local ActivityObj = self:GetObjectsInGroup{ group = "ActivityObj" ,ignoreSpawners = true }.objects
		storeObjectByName(self, "ActivityObj", ActivityObj[1])
		
		rebuilds = self:GetObjectsInGroup{ignoreSpawners=true, group = "rebuilds" }.objects
	end

	
	local Activity = getObjectByName(self, "ActivityObj")

	
	
	-- Random Player or QB
		local num = math.random(1, 100)
		
		if num > 1 and num < 60 then
		
			HitPlayer = true
			HitQB = false
		else
		
			HitPlayer = false
			HitQB = true
			
		end
		
		if HitPlayer then

		local objects = Activity:GetAllActivityUsers{}.objects

		if objects then
			local ranP = math.random(1, #objects)

			local nump = math.random(1, 100)

				if nump > 1 and num < static['Chance_ToHit_Player'] then
                    if objects[ranP] then
 					    self:CastSkill{skillID = 355 , optionalTargetID = objects[ranP] } 
 					end
				else


					local Markpos = objects[ranP]:GetPosition().pos 
					local Markrot = objects[ranP]:GetRotation()	
					local pos = {x = Markpos.x + 5, y = Markpos.y + 5, z = Markpos.z + 5}
					if objects[ranP] then
					    self:CastSkill{skillID = 355 , lastClickedPosit = pos  } 
                    end


				end

			else
				

				local numq = math.random(1, 100)
				local ranp = math.random(1, #rebuilds)
				
				
				 local rebuildState = rebuilds[ranp]:GetRebuildState()
				    
				    -- if the state is idle we are active
   				 if (rebuildState and tonumber(rebuildState.iState) == 3) then
					if numq > 1 and num < static['Chance_ToHit_ReBuild'] then
						if rebuilds[ranp] then
							self:CastSkill{skillID = 355 , optionalTargetID = rebuilds[ranp] } 
						end
					else
						local Markpos = rebuilds[ranp]:GetPosition().pos 
						local Markrot = rebuilds[ranp]:GetRotation()	
						local pos = {x = Markpos.x + 5, y = Markpos.y + 5, z = Markpos.z + 5}
						 if rebuilds[ranp] then
						self:CastSkill{skillID = 355 , lastClickedPosit = pos  } 
						end
					end	
			  	end
			end
		end

end

function onTimerDone(self,msg)

	if msg.name == "fire" then
		self:ShootingGalleryFire()
		if self:GetVar("ON") then
		local num = math.random(4, 8)
		
				GAMEOBJ:GetTimer():AddTimerWithCancel( num , "fire", self )
		end

	end


end

function onNotifyObject(self,msg)


	if msg.name == "ON" then
		self:SetVar("ON", true)
		GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "fire", self )
	elseif msg.name == "OFF" then
		self:SetVar("ON", false)
		GAMEOBJ:GetTimer():CancelAllTimers( self ) 
	end
	
end