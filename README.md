# EinBot
Einbot is a Discord bot built in C# and using the Discord.Net 3.x API.

It is not meant to be run as a global bot, but instead to be hosted by individuals for their own Discord channels.

The purpose of Einbot is to allow Guild owners to create their own "currencies," or collections of currencies, which they can assign to keys, users, or roles, modify, and have custom displays of the collection.

## Collection
A collection is a group of currencies which is assigned to a role in the channel.  It's roughly analagous to a "class" in OOP.
They come in three types: PerKey, PerUser, and PerRole.
### PerKey
PerKey currencies are those which aren't tied to users or roles, but instead instances are tied to a unique key given by the user.
### PerUser
PerUser currencies are those which are tied to a specific user.  There may only be one instance of a collection per user.
### PerRole
PerRole currencies are those which are tied to the role itself.  There can only be one instance of a PerRole currency.

## Currency
A currency is simply a property in a collection which has a "Type" and can contain a value.  For instance, you might have a currency called "Gold" which is of type "Int."  Another one might be "Name" of type "Text."  These can be set (setting the gold to 30 will make the value of Gold 30), or modified (modifying the value of Gold by -10 will subtract 10 from the current value of gold).

## Creating a collection.
Creating a collection is like constructing a blueprint.  You begin by using the /collection create command and assigning a Discord Role and selecting the collection type (PerKey, PerUser, or PerRole).

Once the collection is created you may start adding currencies to it.

### Adding currencies
Currencies are added to collections using the
- /collection currency add [Role] [Data-Type] [Currency Name] command.
The [Role] is the role the currency is assigned to.
The [Data-Type] is what sort of data this currency is (text, decimals, intechers, user ids, &c).
The [Currench Name] is the actual name of the currency.

There are also commands for removing currencies and renaming them.

### Instantiating a collection.
Once the user has created the collection and added some currencies to it, they must instantiate it.  Up until now they've been designing the "blue print" of the collection, instantiating is like making a building from the blueprint.  This is done with the 
- /currency instantiate [Role] [user/key] command.  

[Role] is the role of the collection to instantiate
[User] if the collection type is PerUser, then you should @[UserName] to instantiate a collection for them.
[Key] if the collection type is PerKey, this is the unique key identifier of the collection.

There is also an uninstantiate command to delete a particular instance of the collection (this will not delete the collection itself - just the instance provided).

# Example:

## An AD&D character.
### Creating the collection
First we'll create a role, using Discord, called "AD&DCharacter."
Now we'll create the collection.  Because a user might have multiple characters, we will select PerKey for the collection type.
- /collection create @AD&DCharacter PerKey

### Adding currencies.
Now that we've created the collection, we need to add some currencies to it.  We'll keep this simple for purposes of the example, but this can be as complicated as the user would like.

First we'll add a name currency:
- /collection currency create @AD&DCharacter Text Name

Then an experience currency:
- /collection currency create @AD&DCharacter Int Experience

And a STR currency:
- /collection currency create @AD&DCharacter Decimal STR

Lastly a portrait currency:
- /collection currency create @AD&DCharacter Text Portrait.

### Instantiating and modifying currencies.
Now that we have a basic collection, we'll instantiate it for a character named "Tim."
- /collection instantiate @AD&DCharacter Tim

### Modifying currencies.
An instance's currency values can be set or modified.  Setting a currency overwrites it to the new value, modifying a numeric currency will either add or subtract the given amount (modifying a text currency has the same effect as setting it).

#### We'll start setting Tim's currencies now.
- /currency set @AD&DCharacter Name Tim Tim
- /currency set @AD&DCharacter STR 18.26 Tim
- /currency set @AD&DCharacter Experience 1900 Tim
- /currency set @AD&DCharacter Portrait www.website.com/tim_portrait.png Tim

#### If we wanted to change Tim's character portrait we could either:
 - /currency set AD&DCharacter Portrait www.website.com/tim_portrait2.png Tim
 - /currency modify AD&DCharacter Portrait www.website.com/tim_portrait2.png Tim

#### If we wanted to subtract 200 experience points to Tim's experience we would:
 - /currency modify AD&DCharacter Experience -200 Tim

### Creating a display for collections.
Now that we have a collection we can build a display for it.  Note that you can insert the value of a currency by including the currency name in curly braces.  For instance, "{STR}" would be replaced with "18.26" since the STR currency for Tim is 18.26.

We'll start with a title:
 - /display add @AD&DCharacter Title Character information for {Name}

Now let's add the character portrait.
 - /display add @AD&DCharacter Thumbnail {Portrait}

And the experience and STR.
 - /display add @AD&DCharacter Field Strength {STR}
 - /display add @AD&DCharacter Field EXP {Experience}

Now to view the display we would type /view AD&DCharacter Tim.
To get information about the display we'd use
- /display info @AD&DCharacter

# Conclusion
This concludes the brief uses of this bot.  Note that there are quite a few commands not covered, such as the ability to create collections or modify instances using CSV files.  For more information use the /help command of the bot, or read the source code.
