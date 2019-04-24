
--------------------------------------------------------------
-- include this script if you care when a group of choicebuilds all are set to the same choice
-- the script that includes it will need to:
	-- call SetGroupName so this script knows what group the choicebuilds are in
	-- call InitializeIndices any time the choices are all reset
	-- call SetIndex any time one choice changes.  Pass in true if you want to check if they're all alike.
	-- define function RewardBonus with what you want to happen when all the choicebuilds are the same
--------------------------------------------------------------





--------------------------------------------------------------
-- vars
--------------------------------------------------------------
local INDICES = nil		-- the index of the current choice for each choicebuild




--------------------------------------------------------------
-- remember the name of the group that 
--------------------------------------------------------------
function SetGroupName( self, name )

	self:SetVar( "groupName", name ) 
end





--------------------------------------------------------------
-- set the index for every choicebuild in the group to -1
--------------------------------------------------------------
function InitializeIndices( self )

	-- create the array to store the indices in
	INDICES = {}
	
	
	-- get the group of choicebuilds
	local group = self:GetObjectsInGroup{ group = self:GetVar( "groupName" ), ignoreSpawners = true }.objects
	
	
	-- set the index for each to -1
	for i = 1, #group do
	
		local buildID = group[i]:GetID()
		INDICES[buildID] = -1
		
	end
end





--------------------------------------------------------------
-- returns whether the given choicebuild is in the group
--------------------------------------------------------------
function IsValidBuildID( self, checkID )

	-- get the group of choicebuilds
	local group = self:GetObjectsInGroup{ group = self:GetVar( "groupName" ), ignoreSpawners = true }.objects
	
	
	-- check each one's ID against the one passed in
	for i = 1, #group do
	
		local currentID = group[i]:GetID()
		if ( currentID == checkID ) then
			return true
		end
		
	end
	
	
	return false
	
end





--------------------------------------------------------------
-- sets the index for the given choicebuild to the given value
-- if bCheckForBonus is true, then it also checks whether all the choicebuilds in the group match
--------------------------------------------------------------
function SetIndex( self, buildID, newIndex, bCheckForBonus )

	if ( IsValidBuildID( self, buildID ) == false ) then
		return
	end
	

	INDICES[buildID] = newIndex
	
	
	if ( bCheckForBonus == true ) then
	
		if ( DoAllChoicebuildsMatch( self ) == true ) then
		
			local index = GetIndexOfFirstChoicebuild( self )
			RewardBonus( self, index )		-- NOTE: the script that includes this one is responsible for defining RewardBonus
		end
		
	end
	
end




--------------------------------------------------------------
-- returns the index of the first choicebuild in the group
--------------------------------------------------------------
function GetIndexOfFirstChoicebuild( self )

	local group = self:GetObjectsInGroup{ group = self:GetVar( "groupName" ), ignoreSpawners = true }.objects
	if ( #group == 0 ) then
		return -1
	end
	

	return INDICES[group[1]:GetID()]
	
end





--------------------------------------------------------------
-- returns whether or not all the choicebuilds in the group are set to the same index 
--------------------------------------------------------------
function DoAllChoicebuildsMatch( self )

	-- get the group of choicebuilds
	local group = self:GetObjectsInGroup{ group = self:GetVar( "groupName" ), ignoreSpawners = true }.objects
	local numChoicebuilds = #group
	
	
	-- if we didn't find any choicebuilds, return false
	if ( numChoicebuilds == 0 ) then
		return false
	end
	
	
	-- get the index of the first choicebuild in the group.
	-- we'll compare all the others to that
	local checkIndex = INDICES[group[1]:GetID()]
	if ( checkIndex == -1 ) then
		return false
	end


	-- check whether all the choicebuilds are the same
	for i = 1, numChoicebuilds do
	
		local buildID = group[i]:GetID()
		local choiceIndex = INDICES[buildID]
		if ( choiceIndex ~= checkIndex ) then
			return false
		end
		
	end
	

	return true
		
end