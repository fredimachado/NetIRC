namespace NetIRC
{
    /// <summary>
    /// Represents known numeric IRC replies and errors from the server.
    /// </summary>
    public enum IRCNumericReply
    {
        /// <summary>Unknown numeric reply.</summary>
        UNKNOWN,
        /// <summary>RPL_WELCOME numeric reply.</summary>
        RPL_WELCOME = 001,
        /// <summary>RPL_YOURHOST numeric reply.</summary>
        RPL_YOURHOST = 002,
        /// <summary>RPL_CREATED numeric reply.</summary>
        RPL_CREATED = 003,
        /// <summary>RPL_MYINFO numeric reply.</summary>
        RPL_MYINFO = 004,
        /// <summary>RPL_ISUPPORT numeric reply.</summary>
        RPL_ISUPPORT = 005, // can be RPL_BOUNCE as well :(
        /// <summary>RPL_YOURID numeric reply.</summary>
        RPL_YOURID = 042,
        /// <summary>RPL_USERHOST numeric reply.</summary>
        RPL_USERHOST = 302,
        /// <summary>RPL_ISON numeric reply.</summary>
        RPL_ISON = 303,
        /// <summary>RPL_AWAY numeric reply.</summary>
        RPL_AWAY = 301,
        /// <summary>RPL_UNAWAY numeric reply.</summary>
        RPL_UNAWAY = 305,
        /// <summary>RPL_NOWAWAY numeric reply.</summary>
        RPL_NOWAWAY = 306,
        /// <summary>RPL_WHOISUSER numeric reply.</summary>
        RPL_WHOISUSER = 311,
        /// <summary>RPL_WHOISSERVER numeric reply.</summary>
        RPL_WHOISSERVER = 312,
        /// <summary>RPL_WHOISOPERATOR numeric reply.</summary>
        RPL_WHOISOPERATOR = 313,
        /// <summary>RPL_WHOISIDLE numeric reply.</summary>
        RPL_WHOISIDLE = 317,
        /// <summary>RPL_ENDOFWHOIS numeric reply.</summary>
        RPL_ENDOFWHOIS = 318,
        /// <summary>RPL_WHOISCHANNELS numeric reply.</summary>
        RPL_WHOISCHANNELS = 319,
        /// <summary>RPL_WHOWASUSER numeric reply.</summary>
        RPL_WHOWASUSER = 314,
        /// <summary>RPL_ENDOFWHOWAS numeric reply.</summary>
        RPL_ENDOFWHOWAS = 369,
        /// <summary>RPL_LISTSTART numeric reply.</summary>
        RPL_LISTSTART = 321,
        /// <summary>RPL_LIST numeric reply.</summary>
        RPL_LIST = 322,
        /// <summary>RPL_LISTEND numeric reply.</summary>
        RPL_LISTEND = 323,
        /// <summary>RPL_UNIQOPIS numeric reply.</summary>
        RPL_UNIQOPIS = 325,
        /// <summary>RPL_CHANNELMODEIS numeric reply.</summary>
        RPL_CHANNELMODEIS = 324,
        /// <summary>RPL_NOTOPIC numeric reply.</summary>
        RPL_NOTOPIC = 331,
        /// <summary>RPL_TOPIC numeric reply.</summary>
        RPL_TOPIC = 332,
        /// <summary>RPL_INVITING numeric reply.</summary>
        RPL_INVITING = 341,
        /// <summary>RPL_SUMMONING numeric reply.</summary>
        RPL_SUMMONING = 342,
        /// <summary>RPL_INVITELIST numeric reply.</summary>
        RPL_INVITELIST = 346,
        /// <summary>RPL_ENDOFINVITELIST numeric reply.</summary>
        RPL_ENDOFINVITELIST = 347,
        /// <summary>RPL_EXCEPTLIST numeric reply.</summary>
        RPL_EXCEPTLIST = 348,
        /// <summary>RPL_ENDOFEXCEPTLIST numeric reply.</summary>
        RPL_ENDOFEXCEPTLIST = 349,
        /// <summary>RPL_VERSION numeric reply.</summary>
        RPL_VERSION = 351,
        /// <summary>RPL_WHOREPLY numeric reply.</summary>
        RPL_WHOREPLY = 352,
        /// <summary>RPL_ENDOFWHO numeric reply.</summary>
        RPL_ENDOFWHO = 315,
        /// <summary>RPL_NAMREPLY numeric reply.</summary>
        RPL_NAMREPLY = 353,
        /// <summary>RPL_ENDOFNAMES numeric reply.</summary>
        RPL_ENDOFNAMES = 366,
        /// <summary>RPL_BANLIST numeric reply.</summary>
        RPL_BANLIST = 367,
        /// <summary>RPL_ENDOFBANLIST numeric reply.</summary>
        RPL_ENDOFBANLIST = 368,
        /// <summary>RPL_INFO numeric reply.</summary>
        RPL_INFO = 371,
        /// <summary>RPL_ENDOFINFO numeric reply.</summary>
        RPL_ENDOFINFO = 374,
        /// <summary>RPL_MOTDSTART numeric reply.</summary>
        RPL_MOTDSTART = 375,
        /// <summary>RPL_MOTD numeric reply.</summary>
        RPL_MOTD = 372,
        /// <summary>RPL_ENDOFMOTD numeric reply.</summary>
        RPL_ENDOFMOTD = 376,
        /// <summary>RPL_YOUREOPER numeric reply.</summary>
        RPL_YOUREOPER = 381,
        /// <summary>RPL_REHASHING numeric reply.</summary>
        RPL_REHASHING = 382,
        /// <summary>RPL_TIME numeric reply.</summary>
        RPL_TIME = 391,
        /// <summary>RPL_USERSSTART numeric reply.</summary>
        RPL_USERSSTART = 392,
        /// <summary>RPL_USERS numeric reply.</summary>
        RPL_USERS = 393,
        /// <summary>RPL_ENDOFUSERS numeric reply.</summary>
        RPL_ENDOFUSERS = 394,
        /// <summary>RPL_NOUSERS numeric reply.</summary>
        RPL_NOUSERS = 395,
        /// <summary>RPL_TRACELINK numeric reply.</summary>
        RPL_TRACELINK = 200,
        /// <summary>RPL_TRACECONNECTING numeric reply.</summary>
        RPL_TRACECONNECTING = 201,
        /// <summary>RPL_TRACEHANDSHAKE numeric reply.</summary>
        RPL_TRACEHANDSHAKE = 202,
        /// <summary>RPL_TRACEUNKNOWN numeric reply.</summary>
        RPL_TRACEUNKNOWN = 203,
        /// <summary>RPL_TRACEOPERATOR numeric reply.</summary>
        RPL_TRACEOPERATOR = 204,
        /// <summary>RPL_TRACEUSER numeric reply.</summary>
        RPL_TRACEUSER = 205,
        /// <summary>RPL_TRACESERVER numeric reply.</summary>
        RPL_TRACESERVER = 206,
        /// <summary>RPL_TRACESERVICE numeric reply.</summary>
        RPL_TRACESERVICE = 207,
        /// <summary>RPL_TRACENEWTYPE numeric reply.</summary>
        RPL_TRACENEWTYPE = 208,
        /// <summary>RPL_TRACECLASS numeric reply.</summary>
        RPL_TRACECLASS = 209,
        /// <summary>RPL_TRACERECONNECT numeric reply.</summary>
        RPL_TRACERECONNECT = 210,
        /// <summary>RPL_TRACELOG numeric reply.</summary>
        RPL_TRACELOG = 261,
        /// <summary>RPL_TRACEEND numeric reply.</summary>
        RPL_TRACEEND = 262,
        /// <summary>RPL_STATSLINKINFO numeric reply.</summary>
        RPL_STATSLINKINFO = 211,
        /// <summary>RPL_STATSCOMMANDS numeric reply.</summary>
        RPL_STATSCOMMANDS = 212,
        /// <summary>RPL_ENDOFSTATS numeric reply.</summary>
        RPL_ENDOFSTATS = 219,
        /// <summary>RPL_STATSUPTIME numeric reply.</summary>
        RPL_STATSUPTIME = 242,
        /// <summary>RPL_STATSOLINE numeric reply.</summary>
        RPL_STATSOLINE = 243,
        /// <summary>RPL_UMODEIS numeric reply.</summary>
        RPL_UMODEIS = 221,
        /// <summary>RPL_SERVLIST numeric reply.</summary>
        RPL_SERVLIST = 234,
        /// <summary>RPL_SERVLISTEND numeric reply.</summary>
        RPL_SERVLISTEND = 235,
        /// <summary>RPL_LUSERCLIENT numeric reply.</summary>
        RPL_LUSERCLIENT = 251,
        /// <summary>RPL_LUSEROP numeric reply.</summary>
        RPL_LUSEROP = 252,
        /// <summary>RPL_LUSERUNKNOWN numeric reply.</summary>
        RPL_LUSERUNKNOWN = 253,
        /// <summary>RPL_LUSERCHANNELS numeric reply.</summary>
        RPL_LUSERCHANNELS = 254,
        /// <summary>RPL_LUSERME numeric reply.</summary>
        RPL_LUSERME = 255,
        /// <summary>RPL_ADMINME numeric reply.</summary>
        RPL_ADMINME = 256,
        /// <summary>RPL_ADMINLOC1 numeric reply.</summary>
        RPL_ADMINLOC1 = 257,
        /// <summary>RPL_ADMINLOC2 numeric reply.</summary>
        RPL_ADMINLOC2 = 258,
        /// <summary>RPL_ADMINEMAIL numeric reply.</summary>
        RPL_ADMINEMAIL = 259,
        /// <summary>RPL_TRYAGAIN numeric reply.</summary>
        RPL_TRYAGAIN = 263,
        /// <summary>RPL_LOCALUSERS numeric reply.</summary>
        RPL_LOCALUSERS = 265,
        /// <summary>RPL_GLOBALUSERS numeric reply.</summary>
        RPL_GLOBALUSERS = 266,
        /// <summary>ERR_NOSUCHNICK numeric reply.</summary>
        ERR_NOSUCHNICK = 401,
        /// <summary>ERR_NOSUCHSERVER numeric reply.</summary>
        ERR_NOSUCHSERVER = 402,
        /// <summary>ERR_NOSUCHCHANNEL numeric reply.</summary>
        ERR_NOSUCHCHANNEL = 403,
        /// <summary>ERR_CANNOTSENDTOCHAN numeric reply.</summary>
        ERR_CANNOTSENDTOCHAN = 404,
        /// <summary>ERR_TOOMANYCHANNELS numeric reply.</summary>
        ERR_TOOMANYCHANNELS = 405,
        /// <summary>ERR_WASNOSUCHNICK numeric reply.</summary>
        ERR_WASNOSUCHNICK = 406,
        /// <summary>ERR_TOOMANYTARGETS numeric reply.</summary>
        ERR_TOOMANYTARGETS = 407,
        /// <summary>ERR_NOSUCHSERVICE numeric reply.</summary>
        ERR_NOSUCHSERVICE = 408,
        /// <summary>ERR_NOORIGIN numeric reply.</summary>
        ERR_NOORIGIN = 409,
        /// <summary>ERR_NORECIPIENT numeric reply.</summary>
        ERR_NORECIPIENT = 411,
        /// <summary>ERR_NOTEXTTOSEND numeric reply.</summary>
        ERR_NOTEXTTOSEND = 412,
        /// <summary>ERR_NOTOPLEVEL numeric reply.</summary>
        ERR_NOTOPLEVEL = 413,
        /// <summary>ERR_WILDTOPLEVEL numeric reply.</summary>
        ERR_WILDTOPLEVEL = 414,
        /// <summary>ERR_BADMASK numeric reply.</summary>
        ERR_BADMASK = 415,
        /// <summary>ERR_UNKNOWNCOMMAND numeric reply.</summary>
        ERR_UNKNOWNCOMMAND = 421,
        /// <summary>ERR_NOMOTD numeric reply.</summary>
        ERR_NOMOTD = 422,
        /// <summary>ERR_NOADMININFO numeric reply.</summary>
        ERR_NOADMININFO = 423,
        /// <summary>ERR_FILEERROR numeric reply.</summary>
        ERR_FILEERROR = 424,
        /// <summary>ERR_NONICKNAMEGIVEN numeric reply.</summary>
        ERR_NONICKNAMEGIVEN = 431,
        /// <summary>ERR_ERRONEUSNICKNAME numeric reply.</summary>
        ERR_ERRONEUSNICKNAME = 432,
        /// <summary>ERR_NICKNAMEINUSE numeric reply.</summary>
        ERR_NICKNAMEINUSE = 433,
        /// <summary>ERR_NICKCOLLISION numeric reply.</summary>
        ERR_NICKCOLLISION = 436,
        /// <summary>ERR_UNAVAILRESOURCE numeric reply.</summary>
        ERR_UNAVAILRESOURCE = 437,
        /// <summary>ERR_USERNOTINCHANNEL numeric reply.</summary>
        ERR_USERNOTINCHANNEL = 441,
        /// <summary>ERR_NOTONCHANNEL numeric reply.</summary>
        ERR_NOTONCHANNEL = 442,
        /// <summary>ERR_USERONCHANNEL numeric reply.</summary>
        ERR_USERONCHANNEL = 443,
        /// <summary>ERR_NOLOGIN numeric reply.</summary>
        ERR_NOLOGIN = 444,
        /// <summary>ERR_SUMMONDISABLED numeric reply.</summary>
        ERR_SUMMONDISABLED = 445,
        /// <summary>ERR_USERSDISABLED numeric reply.</summary>
        ERR_USERSDISABLED = 446,
        /// <summary>ERR_NOTREGISTERED numeric reply.</summary>
        ERR_NOTREGISTERED = 451,
        /// <summary>ERR_NEEDMOREPARAMS numeric reply.</summary>
        ERR_NEEDMOREPARAMS = 461,
        /// <summary>ERR_ALREADYREGISTRED numeric reply.</summary>
        ERR_ALREADYREGISTRED = 462,
        /// <summary>ERR_NOPERMFORHOST numeric reply.</summary>
        ERR_NOPERMFORHOST = 463,
        /// <summary>ERR_PASSWDMISMATCH numeric reply.</summary>
        ERR_PASSWDMISMATCH = 464,
        /// <summary>ERR_YOUREBANNEDCREEP numeric reply.</summary>
        ERR_YOUREBANNEDCREEP = 465,
        /// <summary>ERR_YOUWILLBEBANNED numeric reply.</summary>
        ERR_YOUWILLBEBANNED = 466,
        /// <summary>ERR_KEYSET numeric reply.</summary>
        ERR_KEYSET = 467,
        /// <summary>ERR_CHANNELISFULL numeric reply.</summary>
        ERR_CHANNELISFULL = 471,
        /// <summary>ERR_UNKNOWNMODE numeric reply.</summary>
        ERR_UNKNOWNMODE = 472,
        /// <summary>ERR_INVITEONLYCHAN numeric reply.</summary>
        ERR_INVITEONLYCHAN = 473,
        /// <summary>ERR_BANNEDFROMCHAN numeric reply.</summary>
        ERR_BANNEDFROMCHAN = 474,
        /// <summary>ERR_BADCHANNELKEY numeric reply.</summary>
        ERR_BADCHANNELKEY = 475,
        /// <summary>ERR_BADCHANMASK numeric reply.</summary>
        ERR_BADCHANMASK = 476,
        /// <summary>ERR_NOCHANMODES numeric reply.</summary>
        ERR_NOCHANMODES = 477,
        /// <summary>ERR_BANLISTFULL numeric reply.</summary>
        ERR_BANLISTFULL = 478,
        /// <summary>ERR_NOPRIVILEGES numeric reply.</summary>
        ERR_NOPRIVILEGES = 481,
        /// <summary>ERR_CHANOPRIVSNEEDED numeric reply.</summary>
        ERR_CHANOPRIVSNEEDED = 482,
        /// <summary>ERR_CANTKILLSERVER numeric reply.</summary>
        ERR_CANTKILLSERVER = 483,
        /// <summary>ERR_RESTRICTED numeric reply.</summary>
        ERR_RESTRICTED = 484,
        /// <summary>ERR_UNIQOPPRIVSNEEDED numeric reply.</summary>
        ERR_UNIQOPPRIVSNEEDED = 485,
        /// <summary>ERR_NOOPERHOST numeric reply.</summary>
        ERR_NOOPERHOST = 491,
        /// <summary>ERR_UMODEUNKNOWNFLAG numeric reply.</summary>
        ERR_UMODEUNKNOWNFLAG = 501,
        /// <summary>ERR_USERSDONTMATCH numeric reply.</summary>
        ERR_USERSDONTMATCH = 502
    }
}
