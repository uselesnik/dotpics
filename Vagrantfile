# -*- mode: ruby -*-
# vi: set ft=ruby :

# All Vagrant configuration is done below. The "2" in Vagrant.configure
# configures the configuration version (we support older styles for
# backwards compatibility). Please don't change it unless you know what
# you're doing.
Vagrant.configure("2") do |config|
  # The most common configuration options are documented and commented below.
  # For a complete reference, please see the online documentation at
  # https://docs.vagrantup.com.

  # Every Vagrant development environment requires a box. You can search for
  # boxes at https://vagrantcloud.com/search.
  config.vm.box = "cloud-image/ubuntu-24.04"
  config.vm.box_version = "20251031.0.0"
  config.vm.box_check_update = false

  # config.vm.network "forwarded_port", guest: 80, host: 80
  # config.vm.network "forwarded_port", guest: 443, host: 443

  # config.vm.synced_folder "../data", "/vagrant_data"

  config.vm.synced_folder ".", "/vagrant", disabled: true

  config.vm.provider "virtualbox" do |vb|
    # Display the VirtualBox GUI when booting the machine
    vb.gui = false
  
    # Customize the amount of memory on the VM:
    vb.memory = "4096"
    vb.cpus = 4
  end
  config.vm.provision "shell", inline: <<-SHELL
    sudo -i
    apt-get update
  SHELL
end
