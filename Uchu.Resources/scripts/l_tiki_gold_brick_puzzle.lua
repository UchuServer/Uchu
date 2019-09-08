require('o_mis')

local gemsShot = 0
local firstShooter = ""
local target = ""
--local bouncer1Obj = ""
local bouncer2Obj = ""
local bouncer3Obj = ""
local bouncer4Obj = ""

function onStartup(self, msg)
--    bouncer1Obj = self:GetObjectsInGroup{ group = "TikiBouncer01" }.objects
    bouncer2Obj = self:GetObjectsInGroup{ group = "TikiBouncer02" }.objects
    bouncer3Obj = self:GetObjectsInGroup{ group = "TikiBouncer03" }.objects
    bouncer4Obj = self:GetObjectsInGroup{ group = "TikiBouncer04" }.objects

end

function onFireEvent( self, msg )
    local var =  split(msg.args, ',')
    print (var[1])
    print (var[2])
	if ( msg.args ) then
		if self:GetVar("GemShot") == nil then
		    if ( gemsShot == 0 ) then
		                --print( bouncer1Obj[1])
				   print("*****triggered fireEvent path movement **************")
				   GAMEOBJ:GetTimer():AddTimerWithCancel( 50 , "GemTimer", self )
				   firstShooter = var[2]
		        playersBounce( self, msg )
		    end
		    gemsShot = gemsShot + 1
		    print("****************"..gemsShot.."*****************")
		    if ( gemsShot >= 4 ) then
		        playersBounce( self, msg )
		        gemsShot = 0
		        GAMEOBJ:GetTimer():CancelTimer("GemTimer", self);
		    end        
				   
		end
	end
end

function playersBounce (self, msg)
    local bouncerObj = {}
    if ( firstShooter == "G1" ) then
        bouncerObj = self:GetObjectsInGroup{ group = "TikiBouncer01", ignoreSpawner = true}.objects
        idString = bouncerObj[1]:GetLOT().objtemplate
        print(idString)
    elseif ( firstShooter == "G2" ) then
        target = bouncer2Obj
    elseif ( firstShooter == "G3" ) then
        target = bouncer3Obj
    elseif ( firstShooter == "G4" ) then
        target = bouncer4Obj
    end


    print("************ BouncerPlayers in order "..firstShooter.." first ****************")
    --bouncerObj[1]:BouncerActiveStatus{bActive = true}
    --self:BouncerTriggered{triggerObj = idString}
    --GAMEOBJ:DeleteObject( bouncerObj[1] ) 
        self:BouncePlayer{ ObjIDBouncer = bouncerObj[1] }

end

function BouncePlayers( self )

	local objs = self:GetProximityObjects().objects
	local index = 1

	while index <= table.getn(objs)  do
		local target = objs[index]
		local faction = target:GetFaction()
		
		--verify that we are only bouncing players
		if target:BelongsToFaction{factionID = 1}.bIsInFaction then
		
			--print ( "------------------------------------------------------------------" )
			--print ( "hazmat bouncer BouncePlayers: found a player on the bouncepad" )
			--print ( "------------------------------------------------------------------" )
			
			self:BouncerTriggered{triggerObj = target}
		end
		index = index + 1
	end

end

function onTimerDone(self, msg)
    if msg.name == "GemTimer" then
        if ( gemsShot >= 4 ) then
            print("************* 4 Gems Shot ************")
            playersBounce( self, msg )
        end
        gemsShot = 0
    end
end
