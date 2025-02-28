<br />
<div align="center">
  <a href="https://shorturl.at/kwkso">
    <img src="images/logo.png" alt="Logo" width="80" height="80">
  </a>
  <h3 align="center">Multi-Bookmaker Real-Time Horse Racing Odds Aggregator</h3>
</div>

## About The Project
A few years ago, I discovered arbitrage betting—a method where you place bets on all possible outcomes of an event to guarantee a profit. For instance, in a basketball game offering fixed odds of 50-to-1 for both teams, wagering one dollar on each team ensures a profit of $48. Motivated by this, I developed a system to aggregate real-time horse racing data from multiple bookmakers to identify arbitrage opportunities.

#### Fully Supported Platforms
- Boombet
- colossalbet
- pointsbet
- sportsbett

## Challenges
### Data Aqusition
This involved reverse-engineering each bookmaker’s front end to emulate their real-time data delivery, as well as recreating the processes for generating authentication tokens and constructing API calls needed to access horse racing data from events across Australia. As a result, I now have near-instant access to the odds for every entrant in every race at each meeting, enabling me to run simulations and refine a profitable betting strategy.

### OOP Design
I've undergone multiple revisions of this project, with varying levels of design quality. This GitHub repository represents the most up-to-date version of the project, designed according to OOP principles with varying degrees of adherence. A typical example of OOP in this system is how entrant data is saved to a persistent system. There is a database singleton, which accepts a database-type interface. These database types allow for a variety of different database standards to be incorporated into the system while conforming to a standard API that the rest of the system can reference.

### Rate Limiting
Bookmakers typically don’t want users to scrape large quantities of their public-facing odds data. Rate limiting prevents rudimentary systems from pulling data in the long term. A common technique to circumvent this is simply reducing the number of requests being made. However, that would directly undermine the up-to-date nature of the odds, which I aim to capture. Therefore, I needed to distribute traffic across multiple IP addresses.

There are proxy services that allow for traffic to be distributed in this manner, but they are expensive. Using a VPN is a more affordable alternative; however, most VPN services limit you to communicating through a single tunnel, which will eventually be rate-limited as well. Fortunately, many VPN services allow connections over the OpenVPN standard using account-associated configuration files. Furthermore, the number of connections that can be made using this OpenVPN standard is not limited.

I decided to download configuration files for multiple VPN locations, associating each with a Docker container running a proxy server that routes outgoing traffic through an OpenVPN connection. I can then direct traffic to these containers, enabling an effective and affordable distribution of traffic across multiple distinct IP addresses. Look in the concreteImplementations/re-router folder to see relevant files.

### Built With

<a href="https://www.python.org/">
  <img src="https://img.shields.io/badge/Python-3776AB?style=for-the-badge&logo=python&logoColor=white" alt="Python">
</a>
<a href="">
  <img src="https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white" alt="c#">
</a>
<a href="">
  <img src="https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white" alt="c#">
</a>
<a href="">
  <img src="https://img.shields.io/badge/PowerShell-%235391FE.svg?style=for-the-badge&logo=powershell&logoColor=white" alt="c#">
</a>

## Getting Started
This is an example of how you may give instructions on setting this project locally. Clone this repo and then perform the following steps. 

### Prerequisites

https://dotnet.microsoft.com/en-us/download

1: Clone the repo
2: Install required librarys
  ```sh
  dotnet restore
  ```
3: Build the project
  ```sh
  dotnet build
  ```
4: Run the project
  ```sh
  dotnet run
  ```

## Contact
William Walker - william.sinclair.walker@gmail.com



